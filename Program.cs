using System;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Windows;
using System.Timers;
using System.Threading;
using Timer = System.Timers.Timer;

internal class Program
{
    //===========================
    // Global Variables Section
    //===========================
    static int roundCounter = 0;
    static int myScore = 300;
    static int aiScore = 50;
    static int myTotal = 0;
    static int aiTotal = 0;
    static int cursorRow = Console.WindowHeight / 2;
    static int cursorCol = Console.WindowWidth / 2;
    static Timer timer;
    static bool elapsed = false;
    static int bonusHit = 0;

    //==========================================
    // Title of the console application and flow
    //==========================================
    private static void Main()
    {
        //Title of the console
        Console.Title = "DICE ADVENTURERS";
        Intro();
        StartMenu();
    }

    //===========================
    // Start menu selection
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
                    IntroAdventure();
                    break;
                case 2:
                    IdlePlay();
                    break;
                case 3:
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
        //code for splash screen
        const int MINUS_WIDTH = 10;
        const int MINUS_HEIGHT = 8;
        const int DIV = 2;

        // DICE ADVENTURERS would be in black on a red background in the middle of the screen
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.Black;
        const string S = "DICE ADVENTURERS";

        //position of the title
        Console.SetCursorPosition( Console.WindowWidth/ DIV - MINUS_WIDTH, Console.WindowHeight / DIV - MINUS_HEIGHT);
        Console.WriteLine(S);
        Pause(2000);

        //resetting colors for the next text to come
        Console.ResetColor();
        Console.Clear();

    }

    //===========================
    // Pause for n milliseconds
    //===========================
    static void Pause(int time)
    {
        //pause for the amount of time entered
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
        Console.Clear();
    }

    //===========================
    // Intro for adventure mode
    //===========================
    static void IntroAdventure()
    {
        //this text will appear before starting the game
        Console.Clear();
        Console.WriteLine("\nDuring one villain's long long long long long long long and tedious battle monologue...");
        Pause(4000);
        Adventure();
    }

    //===========================
    // Flow of the adventure game
    //===========================
    static void Adventure()
    {
        Console.Clear();

        //level progression
        //Continue game until one of the scores reaches 0
        while (aiScore >= 0 || myScore >= 0)
        {
            Level();
            EndGame();
            BossFight();
            roundCounter++;
        } 
        
        EndGame();
        End();
        
    }
    
    //=======================================
    // Adventure levels
    //=======================================
    static void Level()
    {
        HealthDisplay();

        //Villain talks
        Console.WriteLine("\nVillain: " + Prompt());
        Pause(1000);

        Console.WriteLine("\nUse the arrow keys to get to the dice\nRoll to attack the Villain\nPress Q to quit");
        Pause(1000);

        //Start timer
        Countdown();

        //range to initiate get die
        const int MIN_WIDTH = 2;
        const int MAX_WIDTH = 6;
        elapsed = false; 

        while (!elapsed)
        {
            {
                //Dice sprite and the corresponding Index
                (string sprite, int indexSprite) levelOne = DiceSprite();
                string diceSprite = levelOne.sprite;
                int spriteIndex = levelOne.indexSprite;

                //Random dice coordinates
                int diceWidth = CoordinatesWidth();
                int diceHeight = CoordinatesHeight();
                bool coord;

                //dice will generate at this position
                Console.SetCursorPosition(diceWidth, diceHeight);
                Console.Write(levelOne.sprite);

                //start game at this position
                Console.SetCursorPosition(cursorCol, cursorRow);

                do
                {
                    Keys();

                    //Cursor on dice check
                    coord = cursorRow == diceHeight && cursorCol <= diceWidth + MAX_WIDTH && cursorCol >= diceWidth - MIN_WIDTH;

                    //exit as soon as timer elapses
                    if(elapsed)
                    {
                        break;
                    }

                } while (!coord);

                Console.Clear();

                //Bonus hit
                bonusHit += levelOne.indexSprite;
                Pause(1000);
            }
        }
 
        Console.WriteLine($"You got a total Bonus of {bonusHit}.");
        Pause(1000);

        //substract total bonus from aiScore
        aiScore -= bonusHit;
        EndGame();
        
        Pause(4000);
        Console.Clear();
    }

    //=======================================
    // Fight for adventure mode
    //=======================================
    static void BossFight()
    {
    
    // ==================================
    // 5 turns to play. Two dice are rolled.
    // The total is subtract from HP.
    // Once one reaches 0 game over
    // ====================================

        int turns;

        Console.WriteLine(@$"
    You have 5 turns to attack the Villain
    Press Enter to Roll");
            
        //wait for Enter input
         WaitForKey(ConsoleKey.Enter);

        for (turns = 5; turns > 0; turns--)
        {
            
            //Calculate score
            Console.Clear();
            HealthDisplay();

            //Count of dice rolls left
            Console.WriteLine($"\nYou have {turns} rolls left");
            Pause(1000);

            //roll the dice
            int myRollOne = DiceRoll();
            int myRollTwo = DiceRoll();
            int aiRollOne = DiceRoll();
            int aiRollTwo = DiceRoll();

            //Calculate totals
            myTotal = myRollOne + myRollTwo;
            aiTotal = aiRollOne + aiRollTwo;

            Console.WriteLine(@$"
        You rolled a {myRollOne} and a {myRollTwo} with a total of {myTotal}
           
        The Villain rolled a {aiRollOne} and a {aiRollTwo} with a total of {aiTotal}");

            //Display a message based on who landed better score
            if (myTotal > aiTotal)
            {
                Pause(1000);
                Console.WriteLine("\nYou landed a good hit!");
            }

            else if (myTotal < aiTotal)
            {
                Pause(1000);
                Console.WriteLine("\nYou were clumsy!");
            }
            else
            {
                Pause(1000);
                Console.WriteLine("\nYou both rolled the same numbers.");
            }

            //wait for Enter and check for end game condition
            Console.WriteLine("\nPress Enter to continue");
            WaitForKey(ConsoleKey.Enter);
            EndGame();
        }

            Console.Clear();

            HealthDisplay();

            //Final message based on who had a better game
            if (myScore > aiScore)
            {
                Pause(1000);
                Console.WriteLine("\nYou had a good fight!\nPress Enter");
                WaitForKey(ConsoleKey.Enter);
            }
            else
            {
                Pause(1000);
                Console.WriteLine("You call this a fight?!!\nPress Enter");
                WaitForKey(ConsoleKey.Enter);
            }
            Console.Clear();
    }
    
    //=======================================
    // End game conditions
    //=======================================
    static void EndGame()
    {
        if (myScore <= 0 || aiScore <= 0)
        {
            Console.Clear();
            HealthDisplay();
            if (myScore < aiScore)
            {
                Console.WriteLine($"\nYOU DIED\n Better luck next time.\n You played {roundCounter} rounds.");
                End();
                Main();
            }
            else if (myScore > aiScore)
            {
                Console.WriteLine($"\nYou fought a valiant battle\nYOU WIN!\nYou won in {roundCounter} rounds.");
                End();
                Main();
            }
        }

    }

    //=======================================
    // Handles scores in BossFight
    //=======================================
    static void ScoreHandler()
    {
        aiScore -= aiTotal;
        myScore -= myTotal;

        //to not display negative score
        if (aiScore < 0)
        {
            aiScore = 0;
        }
        if (myScore < 0)
        {   
            myScore = 0;
        } 
    }
    
    //=======================================
    // Displays Health
    //=======================================
    static void HealthDisplay()
    {
        //to not display negative score
        ScoreHandler();

        Console.WriteLine($"Your Health :     {myScore}\nVillain's Health: {aiScore}");

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
    // START TIMER
    //=======================================
    static void Countdown()
    {
        //the timer will run x milliseconds
        const int SECOND_IN_MILLISECOND = 15000;
        const int MILLISECONDS_IN_SECOND = 1000;
        Console.WriteLine($"\nRush to get as many dice as you can in {SECOND_IN_MILLISECOND / MILLISECONDS_IN_SECOND} seconds for a Bonus Hit!\nPress Enter");
        
        timer = new Timer(SECOND_IN_MILLISECOND);
        timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

        WaitForKey(ConsoleKey.Enter);
        Console.Clear();
        timer.Start();
    }

    //=======================================
    // END TIMER
    //=======================================
 static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // End condition 
            elapsed = true;
            timer.Stop();

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
                    cursorRow--;
                    break;
                case ConsoleKey.DownArrow:
                    cursorRow++;
                    break;
                case ConsoleKey.RightArrow:
                    cursorCol++;
                    break;
                case ConsoleKey.LeftArrow:
                    cursorCol--;
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
            if (cursorRow == minHeightWidth)
            {
                cursorRow = maxHeight;
                cursorRow = maxHeight;
            }
            else if (cursorCol == minHeightWidth)
            {
                cursorCol = maxWidth;
            }
            else if (cursorRow == maxHeight)
            {
                cursorRow = minHeightWidth;
            }
            else if (cursorCol ==maxWidth)
            {
                cursorCol = minHeightWidth;
            }

            Console.SetCursorPosition(cursorCol, cursorRow);
    }

    //=======================================
    // Dice images
    //=======================================
    static public (string, int)  DiceSprite()
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
    // Fight for Idle Play
    //=======================================
    static void IdlePlay()
    {
        Console.Clear();
        //set the health for idle mode
        aiScore = 100;
        myScore = 100;
    // ==================================
    // 5 turns to play. Two dice are rolled.
    // The total is substracted from HP.
    // Once one reaches 0 game over
    // ====================================

        Console.WriteLine("\nRoll to attack the Villainl.\nPress Enter to Roll");
            
        //wait for Enter input
         WaitForKey(ConsoleKey.Enter);

        while (aiScore > 0 || myScore > 0)
        {
            
            //check for end game condition and display health
            EndGame();
            Console.Clear();
            HealthDisplay();
            Pause(1000);

            int myRollOne = DiceRoll();
            int myRollTwo = DiceRoll();
            int aiRollOne = DiceRoll();
            int aiRollTwo = DiceRoll();

            //Calculate totals
            myTotal = myRollOne + myRollTwo;
            aiTotal = aiRollOne + aiRollTwo;

            Console.WriteLine(@$"
        You rolled a {myRollOne} and a {myRollTwo} with a total of {myTotal}
           
        The Villain rolled a {aiRollOne} and a {aiRollTwo} with a total of {aiTotal}");

            //Display a message based on who landed better score
            if (myTotal > aiTotal)
            {
                Pause(1000);
                Console.WriteLine("\nYou landed a good hit!\nPress Enter to contiue");
            }

            else if (myTotal < aiTotal)
            {
                Pause(1000);
                Console.WriteLine("\nYou were clumsy!\nPress Enter to contiue");
            }
            else
            {
                Pause(1000);
                Console.WriteLine("\nYou both rolled the same numbers.\nPress Enter to contiue");
            }
            WaitForKey(ConsoleKey.Enter);
        }

            Console.Clear();

            HealthDisplay();

            //Final message based on who had a better game
            if (myScore > aiScore)
            {
                Pause(1000);
                Console.WriteLine("\nYou had a good fight!\nPress Enter");
                WaitForKey(ConsoleKey.Enter);
            }
            else
            {
                Pause(1000);
                Console.WriteLine("You call this a fight?!!\nPress Enter");
                WaitForKey(ConsoleKey.Enter);
            }
            End();
            Main();
    }
    
    //=======================================
    // Game credits
    //=======================================
    static void Credits()
    {
        Console.Clear();
        const int MINUS_WIDTH = 10;
        const int MINUS_HEIGH_TITLE = 8;
        const int MINUS_HEIGH_NAME = 6;
        const int DIV = 2;

        string[] credits = {"EXECUTIVE PRODUCER", "PRODUCER","STORY BY", "DESIGNER", "UI ARTIST", "LEAD LEVEL DESIGNER",
            "LEAD ENGINEER", "QUALITY ASSURANCE", "PLAYTESTER", "SPECIAL THANKS:"};
        Intro();
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.Black;

        //display each element of the array in the middle of screen
        foreach (string c in credits)
        {
            Console.SetCursorPosition(Console.WindowWidth / DIV - MINUS_WIDTH, Console.WindowHeight / DIV - MINUS_HEIGH_TITLE);
            Console.WriteLine("{0} ", c);
            Console.SetCursorPosition(Console.WindowWidth / DIV - MINUS_WIDTH, Console.WindowHeight / DIV - MINUS_HEIGH_NAME);
            Console.WriteLine("Veronika Vilenski");
            Pause(2000);
            Console.Clear();
        }
        Console.ResetColor();
        Console.Clear();
    }
}



