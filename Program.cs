namespace AP_World_Project;

public class Program {
    private static Dictionary<Topic, string> _infoCorr = new();

    private static Dictionary<string, string> _pirates = new Dictionary<string, string>() {
        ["p"] = "Political", ["i"] = "Intellectual", ["r"] = "Religious", ["a"] = "Artistic", ["t"] = "Technological"
        , ["e"] = "Economic", ["s"] = "Social"
    };

    public static void Main() {
        var program = new Program();
        program.Populate(@"W:\AP World Project\AP World Project\TopicMappings.csv");

        while (true) {
            program.MainMenu();

            var input = Console.ReadLine();

            switch (input.ToLower()) {
                case "1":
                    program.Quiz();
                    break;
                case "2":
                    Flashcards();
                    break;
                case "quit":
                    return;
                case "dev":
                    DisplayDevConsole("brandy2010");
                    break;
                default:
                    continue;
            }
        }
    }


    private void Populate() {
        try {
            var sr = new StreamReader("W:\\AP World Project\\AP World Project\\TopicMappings.csv");
            string? line;
            do {
                line = sr.ReadLine();
                var tempInfo = line.Split(",", 3);

                _infoCorr.Add(new Topic(tempInfo[0].Trim('\"', ','), tempInfo[1].Substring(2, 1))
                    , tempInfo[2].Trim('\"', ','));
            } while (line != null);

            sr.Close();


        }
        catch (Exception e) {
            Console.WriteLine($"Exception: {e}");
        }
    }
    
    private void Populate(string filepath) {
        try {
            var sr = new StreamReader(filepath);
            string? line;
            do {
                line = sr.ReadLine();
                var tempInfo = line.Split(",", 3);

                _infoCorr.Add(new Topic(tempInfo[0].Trim('\"', ','), tempInfo[1].Substring(2, 1))
                    , tempInfo[2].Trim('\"', ','));
            } while (line != null);

            sr.Close();


        }
        catch (Exception e) {
            Console.WriteLine($"Exception: {e}");
        }
    }

    private void MainMenu() {
        Console.Clear();
        Console.WriteLine("""
                          1. Quiz
                          2. Flashcards
                          Type quit to exit the program
                          """);
    }

    private void Quiz() {
        Console.Clear();
        Console.WriteLine(
            "Welcome to the Quiz Portion of WorldLet! \nYou will be given different terms, and you will have to pick which of the four answers below. Type quit to leave:");

        var streak = 0;
        var score = 0;
        var questionIndex = 0;
        var scoreAmount = 100;
        var incorrect = new Dictionary<Topic, string>();



        while (true) {
            if (questionIndex >= _infoCorr.Keys.ToArray().Length) {
                Console.WriteLine($"You finished all the questions! Would you like to:" +
                                  $"\n1. Go over the ones you missed" +
                                  $"\n2. Return to the main menu");
                var input = Console.ReadLine();

                switch (input) {
                    case "1":
                        DisplayDictionary(incorrect);
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        return;
                    case "2":
                        return;
                }
            }

            string[] answerSet = ["", "", "", ""];
            var topicObj = _infoCorr.Keys.ToArray()[questionIndex];
            var topic = topicObj.GetTopic();
            var pirates = topicObj.GetPirates();
            var correctAnswer = _infoCorr[topicObj];
            answerSet[Random.Shared.Next(0, 4)] = correctAnswer;

            for (int i = 0; i < answerSet.Length; i++) {

                if (answerSet[i].Equals("")) {
                    var answerAttempt = _infoCorr.Values.ToArray()[Random.Shared.Next(0, _infoCorr.Values.Count)];

                    while (true) {
                        if (answerSet.Contains(answerAttempt)) {
                            answerAttempt = _infoCorr.Values.ToArray()[Random.Shared.Next(0, _infoCorr.Values.Count)];
                        }
                        else {
                            answerSet[i] = answerAttempt;
                            break;
                        }
                    }
                }
            }

            Console.Clear();
            Console.WriteLine(
                "Welcome to the Quiz Portion of WorldLet! \nYou will be given different terms, and you will have to pick which of the four answers below. Type quit to leave:");
            Console.WriteLine($"What is {topic}: {_pirates[pirates]}");
            Console.WriteLine($"A: {answerSet[0]}\nB: {answerSet[1]}\nC: {answerSet[2]}\nD: {answerSet[3]}");


            var a = answerSet[0].Equals(correctAnswer);
            var b = answerSet[1].Equals(correctAnswer);
            var c = answerSet[2].Equals(correctAnswer);
            var d = answerSet[3].Equals(correctAnswer);


            var answer = Console.ReadLine();

            switch (answer.ToLower()) {
                case "a":
                    if (a) {
                        streak++;
                        score += scoreAmount * streak;
                        Console.WriteLine("You got the question right!");
                    }
                    else {
                        streak = 0;
                        Console.WriteLine($"The correct answer was {correctAnswer}");
                        incorrect.Add(new Topic(topic, pirates), correctAnswer);
                    }

                    PrintScoreStreak(score, streak);

                    break;

                case "b":
                    if (b) {
                        streak++;
                        score += scoreAmount * streak;
                        Console.WriteLine("You got it!");
                    }
                    else {
                        streak = 0;
                        Console.WriteLine($"The correct answer was {correctAnswer}");
                        incorrect.Add(new Topic(topic, pirates), correctAnswer);
                    }

                    PrintScoreStreak(score, streak);

                    break;

                case "c":
                    if (c) {
                        streak++;
                        score += scoreAmount * streak;
                        Console.WriteLine("Good for you! You got it!");
                    }
                    else {
                        streak = 0;
                        Console.WriteLine($"The correct answer was {correctAnswer}");
                        incorrect.Add(new Topic(topic, pirates), correctAnswer);
                    }

                    PrintScoreStreak(score, streak);

                    break;

                case "d":
                    if (d) {
                        streak++;
                        score += scoreAmount * streak;
                        Console.WriteLine("You got the question!");
                    }
                    else {
                        streak = 0;
                        Console.WriteLine($"The correct answer was {correctAnswer}");
                        incorrect.Add(new Topic(topic, pirates), correctAnswer);
                    }

                    PrintScoreStreak(score, streak);

                    break;

                case "quit":
                    return;
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);

            questionIndex++;
        }
    }

    private static void Flashcards() {
        Console.Clear();
        Console.WriteLine("Under construction");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    private static void PrintScoreStreak(int score, int streak) {
        Console.WriteLine($"Score: {score}\n" +
                          $"Streak: {streak}");
    }

    private static void DisplayDictionary(Dictionary<Topic, string> dict) {
        for (int i = 0; i < dict.Keys.Count; i++) {
            var key = dict.Keys.ToArray()[i];
            var value = dict[key];

            Console.WriteLine($"{key.GetTopic()} is {value}");
        }
    }

    private static void DisplayDevConsole(string password) {
        Console.Clear();
        string? input = null;

        while (true) {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter) {
                break;
            }

            input += key.KeyChar;
        }

        if (input.Equals(password)) {
            while (true) {
                Console.WriteLine("Dev Console:\n" +
                                  "1. Display All Questions\n" +
                                  "2. Display Question Analytics (sorted by category)");

                var key = Console.ReadKey(true).Key;

                switch (key) {
                    case ConsoleKey.D1:
                        Console.Clear();
                        foreach (var pair in _infoCorr) {
                            var topic = pair.Key.GetTopic();
                            var type = _pirates[pair.Key.GetPirates()];
                            var explanation = pair.Value;
                            Console.WriteLine($"Topic: {topic}\n" +
                                              $"Type (Pirates): {type}\n" +
                                              $"Explanation: {explanation}");
                            Console.WriteLine();
                        }

                        break;
                    
                    case ConsoleKey.D2:
                        var count = new int[_pirates.Count];

                        foreach (var pair in _infoCorr) {
                            var type = pair.Key.GetPirates();

                            switch (type) {
                                case "p":
                                    count[0]++;
                                    break;
                                case "i":
                                    count[1]++;
                                    break;
                                case "r":
                                    count[2]++;
                                    break;
                                case "a":
                                    count[3]++;
                                    break;
                                case "t":
                                    count[4]++;
                                    break;
                                case "e":
                                    count[5]++;
                                    break;
                                case "s":
                                    count[6]++;
                                    break;
                                    
                                    
                            }
                            
                            
                        }

                        for (int i = 0; i < _pirates.Values.Count; i++) {
                            Console.WriteLine($"{_pirates.Values.ToArray()[i]} | {count[i]}");
                        }

                        break;
                    
                    default:
                        return;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
        }
    }
}