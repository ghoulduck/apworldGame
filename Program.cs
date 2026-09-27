using AP_World_Project;

public class Program {
    private static Dictionary<Topic, string>? _infoCorr;

    public Program() {
        _infoCorr = new Dictionary<Topic, string>();
    }

    public static void Main() {
        var program = new Program();
        program.Populate(true);

        while (true) {
            program.MainMenu();

            var input = Console.ReadLine();
            
            switch (input?.ToLower())
            {
                case "1":
                    program.Quiz();
                    break;
                case "2":
                    program.Flashcards();
                    break;
                case "quit":
                    Console.WriteLine("\nThanks for studying with WorldLet!");
                    return;
                default:
                    Console.WriteLine("Invalid input. Please try again.");
                    System.Threading.Thread.Sleep(1000);
                    break;
            }
        }
    }
    
    // AI Implementation
    private void Populate(bool check) {
        try {
            var sr = new StreamReader("TopicMappings.csv");
            var line = sr.ReadLine();
        
            while (line != null) {
                var tempInfo = line.Split(new char[] {','}, 3);
            
                if (tempInfo.Length == 3) {
                    var topic = tempInfo[0].Trim();
                    var category = tempInfo[1].Trim();
                    var definition = tempInfo[2].Trim();
                
                    _infoCorr?.Add(new Topic(topic, category), definition);
                }
            
                line = sr.ReadLine();
            }
        
            sr.Close();
        } catch (Exception e) {
            Console.WriteLine($"Exception: {e}");
        }
    }

    public void MainMenu() {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════╗");
        Console.WriteLine("║       Welcome to WorldLet! 🌍      ║");
        Console.WriteLine("║   AP World History Study Tool      ║");
        Console.WriteLine("╚════════════════════════════════════╝\n");
        Console.WriteLine("What would you like to do?");
        Console.WriteLine("  1. Quiz");
        Console.WriteLine("  2. Flashcards");
        Console.WriteLine("  Type 'quit' to exit\n");
        Console.Write("Enter your choice: ");
    }

    public void Quiz() {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════╗");
        Console.WriteLine("║         Quiz Mode 📝               ║");
        Console.WriteLine("╚════════════════════════════════════╝\n");
        Console.WriteLine("You will be given different terms, and you must pick which of the four answers is correct.");
        Console.WriteLine("Scoring: 100 points per correct answer × your current streak!");
        Console.WriteLine("Type 'quit' at any time to return to the main menu.\n");
        System.Threading.Thread.Sleep(2000);

        var totalScore = 0;
        var streak = 0;
        const int pointsPerQuestion = 100;

        var topicArray = _infoCorr?.Keys.ToArray();
        
        if (topicArray == null || topicArray.Length == 0) {
            Console.WriteLine("No topics loaded. Please check your TopicMappings.csv file.");
            System.Threading.Thread.Sleep(2000);
            return;
        }

        while (true) {
            Console.Clear();
            Console.WriteLine($"Total Score: {totalScore}  |  Streak: {streak}\n");

            var randomIndex = Random.Shared.Next(topicArray.Length);
            var topic = topicArray[randomIndex];
            var correctAnswer = _infoCorr?[topic];

            var allAnswers = _infoCorr?.Values.Distinct().ToList();

            if (allAnswers == null || allAnswers.Count < 4) {
                Console.WriteLine("Not enough unique answers in the database to create a quiz.");
                System.Threading.Thread.Sleep(2000);
                break;
            }

            var answerSet = new List<string> { correctAnswer };
            var wrongAnswers = allAnswers.Where(a => a != correctAnswer).ToList();

            while (answerSet.Count < 4) {
                var randomWrongIndex = Random.Shared.Next(wrongAnswers.Count);
                var candidate = wrongAnswers[randomWrongIndex];

                if (!answerSet.Contains(candidate)) {
                    answerSet.Add(candidate);
                }
            }

            answerSet = answerSet.OrderBy(_ => Random.Shared.Next()).ToList();

            Console.WriteLine($"What is: {topic.GetTopic()}\n");
            Console.WriteLine("Select the correct definition:\n");
            Console.WriteLine($"A: {answerSet[0]}\n");
            Console.WriteLine($"B: {answerSet[1]}\n");
            Console.WriteLine($"C: {answerSet[2]}\n");
            Console.WriteLine($"D: {answerSet[3]}\n");
            Console.Write("Your answer (A/B/C/D): ");

            var userInput = Console.ReadLine()?.ToUpper();

            if (userInput == "QUIT") {
                Console.WriteLine($"\nFinal Score: {totalScore}");
                System.Threading.Thread.Sleep(2000);
                break;
            }

            if (userInput != "A" && userInput != "B" && userInput != "C" && userInput != "D") {
                Console.WriteLine("Invalid input. Please enter A, B, C, or D.");
                System.Threading.Thread.Sleep(1500);
                continue;
            }

            int answerIndex = userInput[0] - 'A';
            var userAnswer = answerSet[answerIndex];

            if (userAnswer == correctAnswer) {
                streak++;
                int pointsEarned = pointsPerQuestion * streak;
                totalScore += pointsEarned;
                Console.WriteLine($"\n✓ Correct! You earned {pointsEarned} points! (100 × {streak} streak)");
            } else {
                Console.WriteLine($"\n✗ Incorrect. The correct answer was: {correctAnswer}");
                streak = 0;
                Console.WriteLine("Streak reset to 0.");
            }

            System.Threading.Thread.Sleep(2000);
        }
    }

    public void Flashcards() {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════╗");
        Console.WriteLine("║      Flashcards Mode 🃏            ║");
        Console.WriteLine("╚════════════════════════════════════╝\n");

        var topicArray = _infoCorr?.Keys.ToArray();
        
        if (topicArray == null || topicArray.Length == 0) {
            Console.WriteLine("No topics loaded. Please check your TopicMappings.csv file.");
            System.Threading.Thread.Sleep(2000);
            return;
        }

        var knownCards = new HashSet<int>();
        var currentCardIndex = 0;
        var isFlipped = false;

        while (true) {
            Console.Clear();

            int unknownCount = topicArray.Length - knownCards.Count;
            Console.WriteLine($"Progress: {knownCards.Count}/{topicArray.Length} known  |  {unknownCount} remaining\n");

            if (unknownCount == 0) {
                Console.WriteLine("Congratulations! You've mastered all the flashcards! 🎉");
                System.Threading.Thread.Sleep(2000);
                break;
            }

            var topic = topicArray[currentCardIndex];
            var definition = _infoCorr?[topic];

            Console.WriteLine("┌──────────────────────────────────┐");
            if (isFlipped) {
                Console.WriteLine("│ [DEFINITION]                     │");
                Console.WriteLine("├──────────────────────────────────┤");
                Console.WriteLine($"│ {definition}");
                Console.WriteLine("└──────────────────────────────────┘");
            } else {
                Console.WriteLine("│ [TERM]                           │");
                Console.WriteLine("├──────────────────────────────────┤");
                Console.WriteLine($"│ {topic.GetTopic()}");
                Console.WriteLine("└──────────────────────────────────┘");
            }

            Console.WriteLine("\nOptions:");
            Console.WriteLine("  [SPACE] - Flip card");
            Console.WriteLine("  [N]     - Next card");
            Console.WriteLine("  [K]     - Mark as known");
            Console.WriteLine("  [Q]     - Quit flashcards");

            var key = Console.ReadKey(true).KeyChar.ToString().ToUpper();

            switch (key) {
                case " ":
                    isFlipped = !isFlipped;
                    break;

                case "N":
                    isFlipped = false;
                    do {
                        currentCardIndex = (currentCardIndex + 1) % topicArray.Length;
                    } while (knownCards.Contains(currentCardIndex) && knownCards.Count < topicArray.Length);
                    break;

                case "K":
                    knownCards.Add(currentCardIndex);
                    isFlipped = false;
                    
                    do {
                        currentCardIndex = (currentCardIndex + 1) % topicArray.Length;
                    } while (knownCards.Contains(currentCardIndex) && knownCards.Count < topicArray.Length);
                    break;

                case "Q":
                    Console.WriteLine($"\nYou mastered {knownCards.Count}/{topicArray.Length} flashcards!");
                    System.Threading.Thread.Sleep(2000);
                    return;
            }
        }
    }
}
