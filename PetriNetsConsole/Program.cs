using System;
using System.Collections.Generic;
using System.Linq;

namespace PetriNetsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            int steps = 10;
            //Initial data
            int[,] arrayPostT = {
            { 0, 1, 0, 0, 1 },
            { 1, 0, 0, 0, 0 },
            { 0, 1, 0, 0, 0 },
            { 0, 0, 1, 1, 0 },
            { 0, 0, 0, 0, 1 }
            };

            int[,] arrayPreT = {
            { 1, 0, 1, 1, 0 },
            { 0, 1, 0, 0, 0 },
            { 1, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 1 },
            { 0, 0, 0, 1, 0 }
            };

            //Subtract Post from Pret
            int[,] arrayCT = GetArrayCT(arrayPostT, arrayPreT);

            //First tokens positions
            int[] arrayM0 = { 1, 0, 1, 0, 1 };

            //Initialize variables

            //Positions list
            List<int[]> arrayMList = new List<int[]>();
            //Selected options list
            List<int[]> arraySelectedOptionsList = new List<int[]>();
            //List of traveled paths without repeating
            List<int[]> arrayTraveledOptionsList = new List<int[]>();

            //Add the first tokens position to the list
            arrayMList.Add(arrayM0);
            int currentM = 0;

            //Set the random variable used during the whole process
            Random random = new Random();

            //Main step-loop
            for (int ii = 0; ii < steps; ii++)
            {
                //Get the posible options taking into account the current tonkes position
                List<int[]> availableOptions = GetAvailableOptions(arrayPreT, arrayMList[currentM]);

                //Select a path that hasn't been traveled before
                int[] e = new int[5];
                bool foundAnUntraveledPath = false;
                for (int i = 0; i < availableOptions.Count; i++)
                {
                    bool untraveledPath = true;
                    for (int j = 0; j < arrayTraveledOptionsList.Count; j++)
                    {
                        if (Enumerable.SequenceEqual(availableOptions[i], arrayTraveledOptionsList[j]))
                        {
                            untraveledPath = false;
                            break;
                        }
                    }
                    if (untraveledPath)
                    {
                        foundAnUntraveledPath = true;
                        arrayTraveledOptionsList.Add(availableOptions[i]);
                        e = availableOptions[i];
                        arraySelectedOptionsList.Add(e);
                        break;
                    }
                }
                //There wasn't any new paths, so this will get a random path
                if (!foundAnUntraveledPath)
                {
                    int index = random.Next(availableOptions.Count);
                    e = availableOptions[index];
                    arraySelectedOptionsList.Add(e);
                }
                
                int[] ce = new int[5];

                //Multiplications for Ce
                for (int i = 0; i < arrayCT.GetLength(0); i++)
                {
                    ce[i] = 0;
                    for (int j = 0; j < arrayCT.GetLength(1); j++)
                    {
                        ce[i] += arrayCT[i, j] * e[j];
                    }
                }

                //Sum C*e and the last M array
                int[] arrayM = new int[5];
                for (int i = 0; i < arrayM.Length; i++)
                {
                    arrayM[i] = ce[i] + arrayMList[currentM][i];
                }
                arrayMList.Add(arrayM);
                currentM++;
            }

            //Print final array result
            Console.WriteLine("Petri nets");
            Console.WriteLine("Steps:\n");
            PrintArrayList(arrayMList);
            Console.WriteLine();
            Console.Write("Show steps with selected options? [Y/N]");
            var response = Console.ReadKey().Key;
            Console.WriteLine();
            if (response == ConsoleKey.Y)
            {
                PrintSelectedOptions(arrayMList, arraySelectedOptionsList);
                Console.WriteLine();
                Console.Write("Press any key to exit...");
                Console.ReadKey();
            }
        }

        public static void PrintArrayList(List<int[]> list)
        {
            int counter = 0;
            foreach (int[] item in list)
            {
                string space = " ";
                if (counter < 100)
                {
                    space += " ";
                }
                if (counter < 10)
                {
                    space += " ";
                }
                Console.WriteLine("#" + counter + ":"+ space + ArrayToString(item));
                counter++;
            }
        }

        public static void PrintSelectedOptions(List<int[]> steps, List<int[]> selectedOptions)
        {
            int counter = 0;
            for (int i = 0; i < steps.Count; i++)
            {
                string space = " ";
                if (counter < 100)
                {
                    space += " ";
                }
                if (counter < 10)
                {
                    space += " ";
                }
                Console.WriteLine("#" + counter + ":" + space + ArrayToString(steps[i]));
                if (counter < steps.Count-1)
                {
                    Console.WriteLine();
                    Console.WriteLine("Selected option: " + ArrayToString(selectedOptions[i]));
                }
                counter++;
            }
        }
        public static int[,] GetArrayCT(int[,] arrayPostT, int[,] arrayPreT)
        {
            int[,] arrayCT = new int[5, 5];
            for (int i = 0; i < arrayPostT.GetLength(0); i++)
            {
                for (int j = 0; j < arrayPostT.GetLength(1); j++)
                {
                    arrayCT[i, j] = arrayPostT[i, j] - arrayPreT[i, j];
                }
            }
            return arrayCT;
        }

        public static void PrintArrayCT(int[,] arrayPostT, int[,] arrayPreT)
        {
            int[,] arrayCT = GetArrayCT(arrayPostT, arrayPreT);
            string temp = "";
            for (int i = 0; i < arrayCT.GetLength(0); i++)
            {
                for (int j = 0; j < arrayCT.GetLength(1); j++)
                {
                    temp += "[" + arrayCT[i, j] + "] ";
                }
                Console.WriteLine(temp);
                temp = "";
            }
        }

        public static List<int[]> GetAvailableOptions(int[,] arrayPreT, int[] arrayM)
        {
            List<int[]> availableOptions = new List<int[]>();

            for (int i = 0; i < arrayPreT.GetLength(0); i++)
            {
                bool isPossible = true;
                for (int j = 0; j < arrayPreT.GetLength(1); j++)
                {
                    if (arrayPreT[j, i] > 0)
                    {
                        if (arrayM[j] < arrayPreT[j, i])
                        {
                            isPossible = false;
                        }
                    }
                }
                if (isPossible)
                {
                    int[] option = { 0, 0, 0, 0, 0 };
                    option[i] = 1;
                    availableOptions.Add(option);
                }
                isPossible = true;
            }
            return availableOptions;
        }

        public static void PrintAvailableOptions(List<int[]> availableOptions)
        {
            foreach (int[] option in availableOptions)
            {
                string temp = "";
                for (int i = 0; i < option.Length; i++)
                {
                    temp += "[" + option[i] + "] ";
                }
                Console.WriteLine(temp);
            }
        }

        public static string ArrayToString(int[] array)
        {
            string temp = "";
            for (int i = 0; i < array.Length; i++)
            {
                temp += "[" + array[i] + "] ";
            }
            return temp;
        }
    }
}
