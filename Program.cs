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
    static int aiScore = 500;
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
        const string TITLE = "DICE ADVENTURERS";
        Title(TITLE);
        Intro(TITLE);
        StartMenu();
    }

    static void Title (string prompt)
    {
        Console.Title = prompt;
    }

    static int EnterInt (string prompt)
    {
        int number;
        Console.Write(prompt);
        int.TryParse(Console.ReadLine(), out number);
        return number;
    }
    //===========================
    // Start menu selection
    //===========================
    static void StartMenu()
    {
        int choice;
        const string INVALID_INPUT = "Invalid input, restarting menu...";
        const string MENU = (@"
    1. Play
    2. Idle Play: Boss Fight
    3. Chase The Dice
    4. Credits
    5. Quit the Game

    Please Enter a Choice
");

        do
        {

            choice = EnterInt(MENU);

            if (choice == 5)
            {
                End();
                break;
            }

            switch (choice)
            {
                case 1:
                    Adventure();
                    break;
                case 2:
                    IdlePlay();
                    break;
                case 3:
                    ChaseTheDice();
                    break;
                case 4:
                    Credits();
                    break;
                 default:
                    Console.Beep();
                    Console.Write(INVALID_INPUT);
                    Pause(1000);
                    Console.Clear();
                    break;
            }
        }
        while (choice != 5);
    }

    //===========================
    // Splash screen
    //===========================
    static void Intro(string title)
    {
        //code for splash screen
        const int MINUS_WIDTH = 10;
        const int MINUS_HEIGHT = 8;
        const int DIV = 2;

        // DICE ADVENTURERS would be in black on a red background in the middle of the screen
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.Black;

        //position of the title
        Console.SetCursorPosition( Console.WindowWidth/ DIV - MINUS_WIDTH, Console.WindowHeight / DIV - MINUS_HEIGHT);
        Console.WriteLine(title);
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
        string s;

        // will display countdown
        for (int i = 5; i >= 0; i--)
        {
            s = $"\rThe Game will quit in {i} ";
            Console.Write(s);
            Pause(1000);
        }
        Console.Clear();
    }

    //===========================
    // Intro for adventure mode
    //===========================
    static void IntroAdventure()
    {
        //this text will appear as intro to the game
        Console.Clear();
        string s = "\nDuring one villain's long long long long long long long and tedious battle monologue...";
        Console.WriteLine(s);
        Pause(4000);
    }

    //===========================
    // Flow of the adventure game
    //===========================
    static void Adventure()
    {
        Console.Clear();
        const string STAGE_NAME = "ADVENTURE";
        Title(STAGE_NAME);
        Intro(STAGE_NAME);
        IntroAdventure();
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

        const int pauseTime = 1000;
        HealthDisplay();

        //Villain talks
        const string VILLAIN_TALKS = "\nVillain: ";
        Console.WriteLine(VILLAIN_TALKS + Prompt());
        Pause(pauseTime);

        const string INSTRUCTIONS =  "\nUse the arrow keys to get to the dice";
        Console.WriteLine(INSTRUCTIONS);
        Pause(pauseTime);

        //Start timer and random dice locations
        const int time = 15000;
        Countdown(time);
        DiceRandomChase();
        Console.Clear();
        string bonusCount = $"You got a total Bonus of {bonusHit}.";
        Console.WriteLine(bonusCount);
        Pause(pauseTime);

        //substract total bonus from aiScore
        aiScore -= bonusHit;
        EndGame();
        const string PRESS_ENTER = "\nPress Enter to continue";
        WaitForKey(PRESS_ENTER, ConsoleKey.Enter);

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

        const string STAGE_NAME = "BOSS FIGHT";
        Intro(STAGE_NAME);
        const string PRESS_ENTER = "\nPress Enter to continue";
        int turns = 5;
        string instructions = @$"
    You have {turns} turns to attack the Villain
    Press Enter to Roll
    " + PRESS_ENTER;

        //wait for Enter input
        WaitForKey(instructions, ConsoleKey.Enter);

        for (turns = 5 ; turns > 0; turns--)
        {
            //verify end game conditions
            EndGame();
            Console.Clear();

            //Calculate score
            HealthDisplay();

            //Count of dice rolls left
            string turnsLeft = $"\nYou have {turns} rolls left";
            Console.WriteLine(turnsLeft);
            Pause(1000);

            DiceTotals();
            DiceComparisons();

            //wait for Enter and check for end game condition
            WaitForKey(PRESS_ENTER, ConsoleKey.Enter);
        }

            Console.Clear();
            HealthDisplay();

            //Final message based on who had a better game
            FinalDiceComparison();

            Console.Clear();
    }

    //=======================================
    // End game conditions
    //=======================================
    static void EndGame()
    {
        string youDied = $"\nYOU DIED\nBetter luck next time.\nYou played {roundCounter} rounds.";
        string youWin = $"\nYou fought a valiant battle\nYOU WIN!\nYou won in {roundCounter} rounds.";
        string bothLose = $"\nYou both fell in battle as you landed your last epic hits\nYou played {roundCounter} rounds.";
        const string PRESS_ENTER = "\nPress Enter to exit";

        if (myScore <= 0 || aiScore <= 0)
        {
            Console.Clear();
            HealthDisplay();

            if (myScore < aiScore) Console.WriteLine(youDied);

            else if (myScore > aiScore) Console.WriteLine(youWin);
            
            else Console.Write(bothLose);

            WaitForKey(PRESS_ENTER, ConsoleKey.Enter);
            End();
        }


    }

    //===========================
    // Roll a random dice
    //===========================
    static int DiceRoll()
    {

        Random dice = new();
        int rolls = dice.Next(1, 11);
        return rolls;
    }

    //=======================================
    // Roll the dice and add totals
    //=======================================
    static void DiceTotals()
    {
        int [] rolls = new int[4];

        for(int i = 0; i < 4; i++)
        {
            rolls[i] = DiceRoll();
        }

        myTotal = rolls[0] + rolls[1];
        aiTotal = rolls[2] + rolls[3];

        Console.WriteLine(@$"
        You rolled a {rolls[0]} and a {rolls[1]} with a total of {myTotal}

        The Villain rolled a {rolls[2]} and a {rolls[3]} with a total of {aiTotal}");
    }

    //=======================================
    // Compare who landed better dice
    //=======================================
   static void DiceComparisons()
   {
        if (myTotal > aiTotal) Console.WriteLine("\nYou landed a good hit!");

        else if (myTotal < aiTotal) Console.WriteLine("\nYou were clumsy!");

        else Console.WriteLine("\nYou both rolled the same numbers.");

   }

    //=======================================
    // Compare had a better boss fight
    //=======================================
    static void FinalDiceComparison()
    {
        const string PRESS_ENTER = "\nPress Enter to continue";
        const string GOOD_FIGHT = "\nYou had a good fight!\n" + PRESS_ENTER;
        const string BAD_FIGHT = "\nYou can do better!\n" + PRESS_ENTER;

        if (myScore > aiScore)
        {
            Pause(1000);
            WaitForKey(GOOD_FIGHT, ConsoleKey.Enter);
        }
        else
        {
            Pause(1000);
            WaitForKey(BAD_FIGHT, ConsoleKey.Enter);
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
        string score = $"Your Health :     {myScore}\nVillain's Health: {aiScore}";
        Console.WriteLine(score);

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
static void CountdownTwo(int SECOND_IN_MILLISECOND )
    {

        timer = new Timer(SECOND_IN_MILLISECOND);
        timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        timer.Start();
    }

    //=======================================
    // START TIMER
    //=======================================
    static void Countdown(int SECOND_IN_MILLISECOND )
    {
        //the timer will run x milliseconds
        const int MILLISECONDS_IN_SECOND = 1000;
        string instructions = $"\nRush to get as many dice as you can in {SECOND_IN_MILLISECOND / MILLISECONDS_IN_SECOND} seconds for a Bonus Hit!";
        Console.WriteLine(instructions);

        timer = new Timer(SECOND_IN_MILLISECOND);
        timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

        const string PRESS_ENTER = "\nPress Enter to continue";
        WaitForKey(PRESS_ENTER, ConsoleKey.Enter);
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
        int maxHeight = Console.WindowHeight-1;
        int maxWidth = Console.WindowWidth-1;
        const string ERROR = "quack";

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
                    Console.Clear();
                    End();
                    Main();
                    break;
                default:
                    Console.Beep();
                    Console.Write(ERROR);
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
        const int offSet = 1;
        string[] dice = { "[o]", "[oo]", "[ooo]", "[oooo]", "[ooooo]", "[oooooo]" };
        Random diceRd = new Random();
        int diceSpriteNum = diceRd.Next(ARRAY_MIN, dice.Length);
        string diceSprite = dice[diceSpriteNum];
        int spriteIndex = Array.IndexOf(dice, diceSprite);

        //break down of sprites
        (string, int) sprites;
        sprites.Item1 = diceSprite;
        sprites.Item2 = spriteIndex + offSet;
        return sprites;
    }

    //==================================
    //Wait for key input
    //https://stackoverflow.com/questions/71315422/make-user-press-specific-key-to-progress-in-program
    //==================================
    static void WaitForKey(string prompt, ConsoleKey key, ConsoleModifiers modifiers = default)
{
    Console.Write(prompt);

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
        const string STAGE_NAME = "IDLE PLAY";
        Title(STAGE_NAME);
        Intro(STAGE_NAME);
        Console.Clear();

        //set the health for idle mode and round counter
        aiScore = 100;
        myScore = 100;
        roundCounter = 0;

    // ==================================
    // Two dice are rolled.
    // The total is subtracted from HP.
    // Once one reaches 0 game over
    // ====================================

        HealthDisplay();
        //wait for Enter input
        const string PRESS_ENTER = "\nPress Enter to roll";
        const string INSTRUCTIONS = "\nRoll to attack the Villain.\n" + PRESS_ENTER;
        bool endGame = aiScore > 0 || myScore > 0;
        WaitForKey(INSTRUCTIONS, ConsoleKey.Enter);

        while (endGame)
        {
            roundCounter++;
            Console.Clear();

            //game progress
            DiceTotals();
            DiceComparisons();
            HealthDisplay();
            WaitForKey(PRESS_ENTER, ConsoleKey.Enter);
            EndGame();
        }
    }

    //=======================================
    // Chase the dice game
    //=======================================
    static void ChaseTheDice()
    {

        Console.Clear();
        const int time = 30000;
        const string STAGE_NAME = "DICE CHASE";
        Title(STAGE_NAME);
        Intro(STAGE_NAME);

        //Start timer
        Countdown(time);
        DiceRandomChase();

        Console.Clear();
        string bonus =$"Your total is {bonusHit}.\nPress Enter to continue";
        WaitForKey(bonus, ConsoleKey.Enter);
        Console.Clear();
        EndGame();
    }

    //=======================================
    // Randomize dice location and chase them
    //=======================================
    static void DiceRandomChase()
    {
  
        bonusHit =0;

         //offset to get die
        const int MIN_WIDTH = 2;
        const int MAX_WIDTH = 6;
        elapsed = false;

        while (!elapsed)
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

                if (coord)
                {   
                    Console.Clear();
                    Console.SetCursorPosition(cursorCol, cursorRow);
                    //Bonus hit
                    bonusHit += levelOne.indexSprite;
                    Console.Write($"+ {levelOne.indexSprite}");
                    break;
                }

                // //exit as soon as timer elapses
                if (elapsed) break;


            } while (!coord);
        }
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
        const string MY_NAME = "Veronika Vilenski";
        const string INTRO = "CREDITS";
        string[] credits = {"EXECUTIVE PRODUCER", "PRODUCER","STORY BY", "DESIGNER", "UI ARTIST", "LEAD LEVEL DESIGNER",
            "LEAD ENGINEER", "QUALITY ASSURANCE", "PLAYTESTER", "SPECIAL THANKS:"};
        Title(INTRO);
        Intro(INTRO);
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.Black;

        //display each element of the array in the middle of screen
        foreach (string c in credits)
        {
            Console.SetCursorPosition(Console.WindowWidth / DIV - MINUS_WIDTH, Console.WindowHeight / DIV - MINUS_HEIGH_TITLE);
            Console.WriteLine("{0} ", c);
            Console.SetCursorPosition(Console.WindowWidth / DIV - MINUS_WIDTH, Console.WindowHeight / DIV - MINUS_HEIGH_NAME);
            Console.WriteLine(MY_NAME);
            Pause(2000);
            Console.Clear();
        }
        Console.ResetColor();
        Console.Clear();
    }
}



