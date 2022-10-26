using System;
using System.Drawing;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Windows;

internal class Program
{
    //===========================
    // Global Variables Section
    //===========================
    static int myScore = 300;
    static int aiScore = 500;
    static int row = Console.WindowHeight / 2;
    static int col = Console.WindowWidth / 2;

    //==========================================
    // Title of the console application and flow
    //==========================================
    private static void Main()
    {

        Console.Title = "DICE ADVENTURERS";
        Intro();
        StartMenu();
    }

    //===========================
    // Clear screeen
    //===========================
    static void Clear()
    {
        Console.Clear();
    }

    //===========================
    // Satart menue selection
    //===========================
    static int StartMenu()
    {
        do
        {
            Console.Write(@"
    1. Play
    2. Idle Play
    3. Credits
    4. Quit the Game

    Please Enter a Choice
");
            //if the choice is invalid, set to default case and restart menu
            int.TryParse(Console.ReadLine(), out int choice);

            switch (choice)
            {
                case 1:
                    Clear();
                    IntroAdventure();
                    break;
                case 2:
                    Clear();
                    BossFight();
                    break;
                case 3:
                    Clear();
                    Credits();
                    break;
                case 4:
                    return 0;
                 default:
                    Console.Beep();
                    Console.Write("Invalid input, restarting menu...");
                    Pause(1000);
                    Console.Clear();
                    break;
            }
        }
        while (true);
    }

    //===========================
    // Splash screen
    //===========================
    static void Intro()
    {
        //splash screen
        const int MINUS_WIDTH = 10;
        const int MINUS_HEITH = 8;
        const int DIV = 2;
        // DICE ADVENTURERS would in black on a red background in the middle of the screen
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.Black;
        const string S = "DICE ADVENTURERS";
        Console.SetCursorPosition( Console.WindowWidth/ DIV - MINUS_WIDTH, Console.WindowHeight / DIV - MINUS_HEITH);
        Console.WriteLine(S);
        Pause(2000);
        Console.ResetColor();
        Clear();

    }

    //===========================
    // Pause for n secs
    //===========================
    static void Pause(int time)
    {
        Thread.Sleep(time);
    }

    //===========================
    // Exit countdown
    //===========================
    static void End()
    {

        // will display countdown
        for (int i = 5; i >= 0; i--)
        {
            Console.Write($"\rThe Game will quit in {i} ");
            Pause(1000);
        }
        Clear();
    }

    //===========================
    // Intro for adventure mode
    //===========================
    static void IntroAdventure()
    {
        Console.WriteLine("\nDuring one villain's long long long long long long long and tedious battle monologue...");
        Pause(4000);
        Clear();
        Adventure();
    }

    //===========================
    // Roll a random dice
    //===========================
    static int DiceRoll()
    {
        //new random dice
        Random dice = new();
        int roll = dice.Next(1, 11);
        return roll;
    }

    //===========================
    // Flow of the adventure game
    //===========================
    static void Adventure()
    {
        //level progression
        for (int i = 0; i < 5; i++)
        {
            Level();
            BossFight();
        }
        EndGame();
        End();
    }

    //=======================================
    // Coordinates for the dice sprite height
    //=======================================
    static int CoordinatesWidth()
    {
        int[] numbers = Enumerable.Range(10, 100).ToArray();
        Random coordinates = new();
        int index = coordinates.Next(0, numbers.Length);
        int rdNumber = numbers[index];
        return rdNumber;
    }

    //=======================================
    // Coordinates for the dice sprite width
    //=======================================
    static int CoordinatesHeight()
    {
        int[] numbers = Enumerable.Range(5, 20).ToArray();
        Random coordinates = new();
        int index = coordinates.Next(0, numbers.Length);
        int rdNumber = numbers[index];
        return rdNumber;
    }

    //=======================================
    // Villain dialogue
    //=======================================
    static string Prompt()
    {
        //REF for the speech:https://www.scoopwhoop.com/entertainment/times-villains-made-sense-and-convinced-us-they-were-right/
        string[] talk = {"You (humans) move to an area and you multiply and multiply until " +
                "every natural resource is consumed and the only way you can survive is to spread to another area...",
                " You had a bad day once, am I right?… You had a bad day and everything changed. Why else would you...",
                "And in a supreme act of selfishness shattered history like a rank amateur, " +
                "turned the world into a living hell moments away from destruction and ‘I AM’ the villain?",
                "You have been supplied with a false idol to stop you tearing down this CORRUPT CITY! " +
                "Let me tell you the truth... ",
                "This universe is finite, its resources, finite, if life is left unchecked, life will cease to exist. " +
                "It needs correction… I’m the only one who knows that. At least I’m the only one with the will to act on it!",
                "I said, these human beings were flawed and murderous. And for that..."};
        Random speech = new();
        int index = speech.Next(0, talk.Length);
        string talking = talk[index];
        return talking;
    }

    //=======================================
    // Adventure levels
    //=======================================
    static void Level()
    {
        //Dice sprite and the coresponding Index
        (string sprite,int indexSprite) levelOne = new Program().DiceSprite();
        string diceSprite= levelOne.sprite;
        int spriteIndex = levelOne.indexSprite;

        //Random dice coordinates
        int diceWitdth = CoordinatesWidth();
        int diceHeight = CoordinatesHeight();

        //range to initiate BossFight
        const int MIN_WIDTH = 2;
        const int MAX_WIDTH = 2;

        Console.WriteLine($@"Your Health :     {myScore}
Villain's Health: {aiScore}");

        //Villain talks
        Console.WriteLine("\nVillain: " + Prompt());
        Pause(1000);

        Console.WriteLine("\nUse the arrow keys to get to the dice\nRoll to attack the Villain\nPress Q to quit");
        Pause(1000);

        //dice will generate at this position
        Console.SetCursorPosition(diceWitdth, diceHeight);
        Console.Write(levelOne.sprite);

        //start game at this position
        Console.SetCursorPosition(col, row);
        bool coord;
        do
        {   
            Keys();

            coord = row == diceHeight && col <= diceWitdth + MAX_WIDTH && col >= diceWitdth - MIN_WIDTH ;

        } while (!coord);

        Clear();

        //Bonus hit
        Console.WriteLine($"You get a Bonus Hit of {levelOne.indexSprite}.");
        aiScore -= levelOne.indexSprite;
        Pause(4000);
        Clear();
    }

    //=======================================
    // Navigation keys
    //=======================================
    static void Keys()
    {
        ConsoleKeyInfo key;
        int minHeightWidth =0;
        int maxHeight =Console.WindowHeight-1;
        int maxWidth =Console.WindowWidth-1;
        
        //set the cursor's new position
            key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    row--;
                    break;
                case ConsoleKey.DownArrow:
                    row++;
                    break;
                case ConsoleKey.RightArrow:
                    col++;
                    break;
                case ConsoleKey.LeftArrow:
                    col--;
                    break;
                case ConsoleKey.Q:
                    End();
                    Main();
                    break;
                default:
                    Console.Beep();
                    Console.Write("quack");
                    break;
            }
            //resetting the cursor if user hits boundary
            if (row == minHeightWidth)
            {
                row = maxHeight;
                row = maxHeight;
            }
            else if (col == minHeightWidth)
            {
                col = maxWidth;
            }
            else if (row == maxHeight)
            {
                row = minHeightWidth;
            }
            else if (col ==maxWidth)
            {
                col = minHeightWidth;
            }

            Console.SetCursorPosition(col, row);
    }

    //=======================================
    // Dice images
    //=======================================
    public (string, int)  DiceSprite()
    {
        //array display the image of dice
        const int ARRAY_MIN = 0;
        const int ARRAY_MAX = 5;
        const int offSet = 1;
        string[] dice = { "[o]", "[oo]", "[ooo]", "[oooo]", "[ooooo]", "[oooooo]" };
        Random diceRd = new Random();
        int diceSpriteNum = diceRd.Next(ARRAY_MIN, ARRAY_MAX);
        string diceSprite = dice[diceSpriteNum];
        int spriteIndex = Array.IndexOf(dice, diceSprite);
        (string, int) sprites;
        sprites.Item1 = diceSprite;
        sprites.Item2 = spriteIndex + offSet;
        return sprites;
    }

    //=======================================
    // Fight for adventure mode
    //=======================================
    static void BossFight()
    {

        int myTotal;
        int aiTotal;
        int count;

        //=====================================
        //5 turns to play. Two dice are rolled.
        //The total is substracted from HP.
        //Once one reaches 0 game over
        //====================================

        Console.WriteLine(@$"
    You have 5 turns to attack the Villainl
    Press Enter to Roll");
            
            //wait for Enter input
            ConsoleKeyInfo key = Console.ReadKey();
            WaitForKey(ConsoleKey.Enter);
            for (count = 4; count >= 0; count--)
            {

                if (myScore <= 0 || aiScore <= 0) EndGame();

                int myRollOne = DiceRoll();
                int myRollTwo = DiceRoll();
                int aiRollOne = DiceRoll();
                int aiRollTwo = DiceRoll();

                Clear();
                Console.WriteLine($"You have {count} rolls left");
                Pause(1000);
                myTotal = myRollOne + myRollTwo;
                aiTotal = aiRollOne + aiRollTwo;
                Console.WriteLine(@$"
        You rolled a {myRollOne} and a {myRollTwo} with a total of {myTotal}
           
        The Villain rolled a {aiRollOne} and a {aiRollTwo} with a total of {aiTotal}");

                if (myTotal > aiTotal)
                {
                    Pause(1000);
                    myScore -= aiTotal;
                    aiScore -= myTotal;
                    Console.WriteLine(@$"
        You landed a good hit!

        Your health is {myScore} and The Villain's health is {aiScore}
            
        Press Enter to contiue");
                }
                else if (myTotal < aiTotal)
                {
                    Pause(1000);
                    aiScore -= aiTotal;
                    myScore -= myTotal;
                    Console.WriteLine(@$"
        You were clumsy!

        Your health is {myScore} and The Villain's health is {aiScore}
            
        Press Enter to contiue");
                }
                else
                {
                    Pause(1000);
                    aiScore += aiTotal;
                    myScore += myTotal;
                    Console.WriteLine(@$"
        You both rolled the same numbers.

        Your health is {myScore} and The Villain's health is {aiScore}
            
        Press Enter to contiue");
                }
                WaitForKey(ConsoleKey.Enter);

            }
            Clear();
            if (myScore > aiScore)
            {
                Console.WriteLine(@$"
    You had a good fight!

    Your final health is {myScore}
    The Villain's is {aiScore}");
                Pause(4000);
            }
            else
            {
                Console.WriteLine(@$"
    You call this a fight?!!

    Your final health is {myScore}
    The Villain's is {aiScore}");
                Pause(4000);
            }
        
        Clear();
    }

    //==================================
    //Wait for key input
    //https://stackoverflow.com/questions/71315422/make-user-press-specific-key-to-progress-in-program
    //==================================
    static void WaitForKey(ConsoleKey key, ConsoleModifiers modifiers = default)
{
    while (true)
    {
        var keyInfo = Console.ReadKey(true);
        if (keyInfo.Key == key && keyInfo.Modifiers == modifiers)
            return;
    }
}

    //=======================================
    // End game conditions
    //=======================================
    static void EndGame()
    {
        if (myScore <= 0 || myScore < aiScore)
        {
            Console.WriteLine("\nYOU DIED");
            End();
            Main();
        }
        else if (aiScore <= 0 || myScore > aiScore)
        {
            Console.WriteLine("\nYOU WIN");
            End();
        }

    }

    //=======================================
    // Game credits
    //=======================================
    static void Credits()
    {
        const int MINUS_WIDTH = 10;
        const int MINUS_HEITH_TITLE = 8;
        const int MINUS_HEITH_NAME = 6;
        const int DIV = 2;

        string[] credits = {"EXECUTIVE PRODUCER", "PRODUCER","STORY BY", "DESIGNER", "UI ARTIST", "LEAD LEVEL DESIGNER",
            "LEAD ENGINEER", "QUALITY ASSURANCE", "PLAYTESTER", "SPECIAL THANKS:"};
        Intro();
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.Black;

        //display each element of the array in the middle of screen
        foreach (string c in credits)
        {
            Console.SetCursorPosition(Console.WindowWidth / DIV - MINUS_WIDTH, Console.WindowHeight / DIV - MINUS_HEITH_TITLE);
            Console.WriteLine("{0} ", c);
            Console.SetCursorPosition(Console.WindowWidth / DIV - MINUS_WIDTH, Console.WindowHeight / DIV - MINUS_HEITH_NAME);
            Console.WriteLine("Veronika Vilenski");
            Pause(2000);
            Clear();
        }
        Console.ResetColor();
        Clear();
    }
}


