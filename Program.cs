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
    static int myScore = 350;
    static int aiScore = 500;
    static int myTotal = 0;
    static int aiTotal = 0;
    static int cursorRow = 0;
    static int cursorCol = 0;
    static Timer timer;
    static bool elapsed = false;
    static int bonusHit = 0;
    static string userName = "Placeholder";

    private static void Main()
    {
        //Title of the console
        const string TITLE = "DICE ADVENTURERS";
        Title(TITLE);
        Intro(TITLE);

        //Start menu selection
        UserName();
        StartMenu();
    }

    //==========================================
    // Username user input
    //==========================================
    static string UserName()
    {
        //hide cursor
        Console.CursorVisible = false;

        int pauseTime = 1000;
        const string WELCOME ="\n\n Welcome to your first dice dungeons... ";
        const string ASK_NAME = "\n What is your name, adventurer?\n\n";
        string [] answerField = {"?", "?", "?", "?", "?", ""};

        Console.WriteLine(WELCOME);
        Pause(pauseTime);
        Console.WriteLine(ASK_NAME);
        Pause(pauseTime);
        Console.Write($"  {string.Join(" ", answerField)}");
        ConsoleKeyInfo letter;
        int index = 0;
        const int MAX_INDEX = 5;

        //? will get replaced with letters
        do
        {

            letter = Console.ReadKey(true);

                if (letter.Key == ConsoleKey.Backspace && index > 0)
                {
                    //erase characters
                    index--;
                    answerField[index] = "?";
                    Console.Write($"\r  {string.Join(" ", answerField)}");
                }
                else if (index >= 0 && index < MAX_INDEX && letter.Key != ConsoleKey.Enter )
                {
                    string letterInput = letter.KeyChar.ToString();
                    answerField[index] = letterInput;
                    index++;
                    Console.Write($"\r  {string.Join(" ", answerField)}");
                    
                }
                else continue;

           
        } while (letter.Key != ConsoleKey.Enter);

        string[] userNameArray = new string[index];

        for (int i = 0; i < userNameArray.Length ; i++)
         {
            userNameArray[i] = answerField[i];
         }

        userName = string.Concat(userNameArray);
        return userName;
    }
    
    //==========================================
    // Title of the console application
    //==========================================
    static void Title (string prompt)
    {
        Console.Title = prompt;
    }

    //===========================
    // Start menu selection
    //===========================
    static void StartMenu()
    {
        Console.Clear();
        
        int choice;
        int exitNumber = 5;
        string [] menu = {"1. Play", "2. Idle Play: Boss Fight", "3. Chase The Dice",
        "4. Credits", "5. Quit the Game"};
        
        string MENU = (@$"
    Use Arrows to Make a Choice, {userName}:

    1. Play
    2. Idle Play: Boss Fight
    3. Chase The Dice
    4. Credits
    5. Quit the Game

    Please Enter a Choice
");
            
        do
        {
            Console.Write(MENU);
            choice = MenuKeys(menu);

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
            }
        } while (choice != exitNumber);
        
        EndCountdown();

    }

    //===========================
    // Splash screen
    //===========================
    static void Intro(string title)
    {
        //Hide cursor
        Console.CursorVisible = false;

        //code for splash screen size
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
    static void EndCountdown()
    {
        //Hide cursor
        Console.CursorVisible = false;
        Console.Clear();
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
        //Hide cursor
        Console.CursorVisible = false;

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
         //Hide cursor
        Console.CursorVisible = false;

        const string STAGE_NAME = "ADVENTURE";
        Console.Clear();
        Title(STAGE_NAME);
        Intro(STAGE_NAME);

        string instructions = @"Instructions:

        - Press arrow keys or W/A/S/D to move
        - Press Q to Exit
        - Press Enter to continue

        ";
        WaitForKey(instructions);

        Console.Clear();
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
        EndCountdown();
   
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
        
        //hide cursor
        Console.CursorVisible = false;
        string bonusCount = $"You got a total Bonus of {bonusHit}.";
        Console.WriteLine(bonusCount);
        Pause(pauseTime);

        //substract total bonus from aiScore
        aiScore -= bonusHit;
        EndGame();
        const string PRESS_ENTER = "\nPress Enter to continue";
        WaitForKey(PRESS_ENTER);

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
    // ===================================

        Console.Clear();
        const string STAGE_NAME = "BOSS FIGHT";
        Intro(STAGE_NAME);
        const string PRESS_ENTER = "\nPress Enter to continue";
        int turns = 5;
        string instructions = @$"
    You have {turns} turns to attack the Villain
    Press Enter to Roll
    " + PRESS_ENTER;

        //wait for Enter input
        WaitForKey(instructions);

        //OFFSET FOR CURSOR POSITIONS TO PRINT MESSAGES IN CUSTOM ORDER
        const int OFFSET_TOP = 0;
        const int OFFSET_MIDDLE = 2;
        const int OFFSET_BOTTOM = 9;

        for (turns = 4 ; turns >= 0; turns--)
        {
            //verify end game conditions
            if (myScore <= 0 || aiScore <= 0)
            {
                EndGame();
                break;
            }
           
            Console.Clear();

            
            Console.SetCursorPosition(Console.CursorLeft, OFFSET_MIDDLE);
            //Count of dice rolls left
            string turnsLeft = $"\nYou have {turns} rolls left";
            Console.WriteLine(turnsLeft);
            DiceTotals();
            DiceComparisons();

            Console.SetCursorPosition(Console.CursorLeft, OFFSET_TOP);
            //Calculate score
            ScoreHandler();
            HealthDisplay();
            
            Console.SetCursorPosition(Console.CursorLeft, OFFSET_BOTTOM);
            //wait for Enter and check for end game condition
            WaitForKey(PRESS_ENTER);
        }
            
            Console.Clear();
            HealthDisplay();
            EndGame();
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

            WaitForKey(PRESS_ENTER);
            EndCountdown();
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
            WaitForKey(GOOD_FIGHT);
        }
        else
        {
            Pause(1000);
            WaitForKey(BAD_FIGHT);
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
        
        string score = $"{userName}'s Health :     {myScore}\nVillain's Health: {aiScore}";
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
                "You had a bad day once, am I right? You had a bad day and everything changed. Why else would you...",
                "And in a supreme act of selfishness shattered history like a rank amateur, " +
                "turned the world into a living hell moments away from destruction and ‘I AM’ the villain?",
                "You have been supplied with a false idol to stop you tearing down this CORRUPT CITY! " +
                "Let me tell you the truth... ",
                "This universe is finite, its resources, finite, if life is left unchecked, life will cease to exist. " +
                "It needs correction… I’m the only one who knows that. At least I’m the only one with the will to act on it!",
                "I said, these human beings were flawed and murderous. And for that..."};
        string [] greetings = {"Hey, ", "So, ", "Look around you, ", "You are a hypocrite, ", "You will lose, "};
        Random speech = new();
        int indexT = speech.Next(0, talk.Length);
        int indexG = speech.Next(0, greetings.Length);
        string talking = greetings[indexG] + userName + ". " + talk[indexT];
        return talking;
    }

    //=======================================
    // START TIMER
    //=======================================
    static void Countdown(int secondsInMilliseconds )
    {
        //the timer will run x milliseconds
        const int MILLISECONDS_IN_SECOND = 1000;
        string instructions = $"\nRush to get as many dice as you can in {secondsInMilliseconds/ MILLISECONDS_IN_SECOND} seconds for a Bonus Hit!";
        Console.WriteLine(instructions);

        timer = new Timer(secondsInMilliseconds);
        timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

        const string PRESS_ENTER = "\nPress Enter to continue";
        WaitForKey(PRESS_ENTER);
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
        int minHeightWidth = 0;
        int sizeOffset = 1;
        int maxHeight = Console.WindowHeight;
        int maxWidth = Console.WindowWidth;
        
        const string ERROR = "quack";

        //set the cursor's new position
        key = Console.ReadKey(true);
        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                cursorRow--;
                break;
            case ConsoleKey.W:
                cursorRow--;
                break;
            case ConsoleKey.DownArrow:
                cursorRow++;
                break;
            case ConsoleKey.S:
                cursorRow++;
                break;
            case ConsoleKey.RightArrow:
                cursorCol++;
                break;
            case ConsoleKey.D:
                cursorCol++;
                break;
            case ConsoleKey.LeftArrow:
                cursorCol--;
                break;
            case ConsoleKey.A:
                cursorCol--;
                break;
            case ConsoleKey.Q:
                Console.Clear();
                EndCountdown();
                Environment.Exit(0);
                break;
            default:
                Console.Beep();
                Console.Write(ERROR);
                break;
        }

            
            //resetting the cursor if user hits boundary
            if (cursorRow == minHeightWidth)
            {
                cursorRow = maxHeight - sizeOffset;
            }
            else if (cursorCol == minHeightWidth)
            {
                cursorCol = maxWidth - sizeOffset;
            }
            else if (cursorRow == maxHeight)
            {
                cursorRow = minHeightWidth + sizeOffset;
            }
            else if (cursorCol == maxWidth)
            {
                cursorCol = minHeightWidth + sizeOffset;
            }

            Console.SetCursorPosition(cursorCol, cursorRow);
    }

    static int MenuKeys(string [] menu)
{
        int newIndex = 0;
        int oldIndex = 1;
        int indexOffset = 1;
        cursorRow = 3;
        int oldRow;

        //hide cursor
        Console.CursorVisible = false;
        Console.SetCursorPosition(4, cursorRow);
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write(menu[newIndex]);
        ConsoleKeyInfo key;
        Console.SetCursorPosition(4, cursorRow);
        const int MIN_INDEX = 0;
        const int MAX_INDEX = 4;

        do
        {
            key = Console.ReadKey(true);
            oldRow = cursorRow;
            oldIndex = newIndex;
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    cursorRow--;
                    newIndex --;
                    break;
                case ConsoleKey.DownArrow:
                    cursorRow++;
                    newIndex++;
                    break;
                case ConsoleKey.Enter:
                break;
            }

            if (newIndex < MIN_INDEX)
            {
                newIndex = MAX_INDEX;
            }
            else if (newIndex > MAX_INDEX)
            {
                newIndex = MIN_INDEX;
            }
            else if (oldIndex < MIN_INDEX)
            {
                oldIndex = MAX_INDEX;
            }
            else if (oldIndex > MAX_INDEX)
            {
                oldIndex = MIN_INDEX;
            }

            int minHeight = 3;
            int maxHeight = 7;

            if (cursorRow < minHeight)
            {
                cursorRow = maxHeight;
            }
            else if (cursorRow > maxHeight)
            {
                cursorRow = minHeight;
            }

            //reset color of the previous colored row
            Console.ResetColor();
            Console.SetCursorPosition(4, oldRow );
            Console.Write(menu[oldIndex]);

            //color the selection row
            Console.SetCursorPosition(4, cursorRow);
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(menu[newIndex]);     

        } while (key.Key != ConsoleKey.Enter);

        Console.ResetColor();
        return newIndex + indexOffset;
       
    }

    //=======================================
    // Dice images
    //=======================================
    static public (string[], int)  DiceSprite()
    {

         //array display the image of dice
        const int ARRAY_MIN = 0;
        const int offSet = 1;
        
        string [] one = {"+---------+", "|         |", "|    o    |", "|         |", "+---------+ "};
        string [] two = {"+---------+", "| o       |", "|         |", "|       o |", "+---------+ ", };
        string [] three ={"+---------+", "| o       |", "|    o    |", "|       o |", "+---------+ ",};
        string [] four ={"+---------+", "| o     o |", "|         |",  "| o     o |", "+---------+ ",};
        string [] five ={"+---------+", "| o     o |", "|    o    |", "| o     o |", "+---------+ ",};
        string [] six ={"+---------+", "| o     o |", "| o     o |", "| o     o |", "+---------+ ",};
        string [] [] dice =  {one, two, three, four, five, six};

        Random diceRd = new Random();
        int diceSpriteNum = diceRd.Next(ARRAY_MIN, dice.Length);
        string [] diceSprite = dice[diceSpriteNum];
        int spriteIndex = Array.IndexOf(dice, diceSprite);

        //break down of sprites
        (string[], int) sprites;
        sprites.Item1 = diceSprite;
        sprites.Item2 = spriteIndex + offSet;
        return sprites ;
    }

    //==================================
    //Wait for key input
    //==================================
    static void WaitForKey(string prompt)
{
    Console.Write(prompt);

    do
    {
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
        ConsoleKey enter = ConsoleKey.Enter;
        ConsoleKey quit = ConsoleKey.Q;

        if (keyInfo.Key == enter)
            break;
        if (keyInfo.Key == quit)
        {
            Console.Clear();
            EndCountdown();
            Environment.Exit(0);
        }       
    } while (!Console.KeyAvailable);
}

    //=======================================
    // Fight for Idle Play
    //=======================================
    static void IdlePlay()
    {
        const string STAGE_NAME = "IDLE PLAY";
        Console.Clear();
        Title(STAGE_NAME);
        Intro(STAGE_NAME);

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

        //OFFSET FOR CURSOR POSITIONS TO PRINT MESSAGES IN CUSTOM ORDER
        const int OFFSET_TOP = 0;
        const int OFFSET_MIDDLE = 2;
        const int OFFSET_BOTTOM = 8;

        //wait for Enter input
        const string PRESS_ENTER = "\nPress Enter to roll";
        const string INSTRUCTIONS = "\nRoll to attack the Villain.\n" + PRESS_ENTER;
        bool endGame = false;
        WaitForKey(INSTRUCTIONS);

        while (!endGame)
        {
            roundCounter++;
            Console.Clear();

            //game progress
            Console.SetCursorPosition(Console.CursorLeft, OFFSET_MIDDLE);
            DiceTotals();
            DiceComparisons();

            Console.SetCursorPosition(Console.CursorLeft, OFFSET_TOP);
            ScoreHandler();
            HealthDisplay();

            endGame = myScore <= 0 || aiScore <= 0;

            Console.SetCursorPosition(Console.CursorLeft, OFFSET_BOTTOM);
            WaitForKey(PRESS_ENTER);
            
        } 
        EndGame();
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
        WaitForKey(bonus);
        EndCountdown();

    }

    //=======================================
    // Randomize dice location and chase them
    //=======================================
    static void DiceRandomChase()
    {

        //show cursor
        Console.CursorVisible = true;

        bonusHit = 0;
        elapsed = false;
        bool intersect;
cursorRow = Console.WindowHeight/2;
        cursorCol = Console.WindowWidth/2;
        while (!elapsed)
        {
            //Dice sprite and the corresponding Index
            (string[] sprite, int indexSprite) dice = DiceSprite();
            string[] diceSprite = dice.sprite;
            int spriteIndex = dice.indexSprite;

            //Random dice coordinates
            int diceWidth = CoordinatesWidth();
            int diceHeight = CoordinatesHeight();

            //dice will generate at this position
            for (int i = 0; i < 5; i++)
            {
                Console.SetCursorPosition(diceWidth, diceHeight++);
                Console.Write(dice.sprite[i]);
            }

            //start game at this position
            Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);

            do
            {
               
                intersect = Intersect(diceHeight, diceWidth);

                Keys();

                if (intersect)
                {
                    Console.Clear();
                    Console.SetCursorPosition(cursorCol, cursorRow);

                    //Bonus number
                    bonusHit += dice.indexSprite;
                    Console.Write($"+ {dice.indexSprite}");
                    break;
                }

                //exit as soon as timer elapses
                if (elapsed) break;


            } while (!intersect);
        }

    }

static bool Intersect(int diceHeight, int diceWidth)
{
                const int MAX_HEIGHT = 5;
                const int MIN_HEIGHT = 2;
                const int MAX_WIDTH = 10;
                bool offsetLeft = cursorRow <= diceHeight - MIN_HEIGHT && cursorRow >= diceHeight - MAX_HEIGHT && cursorCol == diceWidth;
                bool offsetRight = cursorRow <= diceHeight - MIN_HEIGHT && cursorRow >= diceHeight - MAX_HEIGHT && cursorCol == diceWidth + MAX_WIDTH;
                bool offsetBottom = cursorRow == diceHeight - MIN_HEIGHT && cursorCol >= diceWidth && cursorCol <= diceWidth + MAX_WIDTH;
                bool offsetTop = cursorRow == diceHeight - MAX_HEIGHT && cursorCol >= diceWidth && cursorCol <= diceWidth + MAX_WIDTH;
                bool intersect = offsetLeft || offsetRight || offsetBottom || offsetTop;
                return intersect;


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



