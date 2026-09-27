
using AP_World_Project;

public class Program {
    private static Dictionary<Topic, string>? _infoCorr;

    public Program() {
        _infoCorr = new Dictionary<Topic, string>();
    }

    public static void Main() {
        var program = new Program();
        program.Populate();

        while (true) {
            program.MainMenu();

            var input = Console.ReadLine();
            
            switch (input.ToLower())
            {
                case "1":
                    program.Quiz();
                    Console.WriteLine("quiz");
                    break;
                case "2":
                    //Flashcards()
                    Console.WriteLine("flashcards");
                    break;
                case "quit":
                    return;
            }
        }
    }
    
    
    private void Populate() {
        try {
            var sr = new StreamReader("W:\\AP World Project\\AP World Project\\TopicMappings.csv");
            var temp = "";
            var lines = 0;
            var line = sr.ReadLine();
            while (line != null) {
                lines++;
                
                temp += line;
                line = sr.ReadLine();
            }

            sr.Close();

            var tempInfo = temp.Split(",");
            
            for (int i = 0; i < tempInfo.Length; i += 3) {
                _infoCorr.Add(new Topic(tempInfo[i], tempInfo[i + 1]), tempInfo[i + 2]);
            }

            // var tempInfo = new string[lines][];
            // for (int i = 0; i < tempInfo.Length; i++) {
            //     tempInfo[i] = new string[3];
            //     for (int g = 0; g < 3; g++) {
            //         
            //     }
            // }
            
        } catch (Exception e) {
            Console.WriteLine($"Exception: {e}");
        }
    }
    
    // Possible implementation for custom question sets
    public void Populate(string filepath) {
        try {
            var sr = new StreamReader(filepath);
            var temp = "";
            var lines = 0;

            var line = sr.ReadLine();
            while (line != null) {
                lines++;
                Console.WriteLine(line);
                temp += line;
                line = sr.ReadLine();
            }

            sr.Close();

            var tempInfo = temp.Split(',');
            
            for (int i = 0; i < tempInfo.Length; i += 3) {
                _infoCorr.Add(new Topic(tempInfo[i], tempInfo[i + 1]), tempInfo[i + 2]);
            }

            // var tempInfo = new string[lines][];
            // for (int i = 0; i < tempInfo.Length; i++) {
            //     tempInfo[i] = 
            // }
            
        } catch (Exception e) {
            Console.WriteLine($"Exception: {e}");
        }
    }

    public void MainMenu() {
        Console.Clear();
        Console.WriteLine("""
                          1. Quiz
                          2. Flashcards
                          Type quit to exit the program
                          """);
    }

    public void Quiz() {
        Console.Clear();
        Console.WriteLine("Welcome to the Quiz Portion of WorldLet! \nYou will be given different terms, and you will have to pick which of the four answers below. Type quit to leave:");

        var streak = 0;
        var score = 0;
        var questionIndex = 0;

            
        while (true) {
            var answerSet = new string[4];
            var topic = _infoCorr.Keys.ToArray()[questionIndex];
            var correctAnswer = _infoCorr[topic];
            answerSet[Random.Shared.Next(0, 4)] = correctAnswer;
            var upper = 5;
            var lower = 0;
            
            for (int i = 0; i < 4;) {
                var current = _infoCorr.Values.ToArray()[Random.Shared.Next(0, _infoCorr.Values.Count)];
                
                if (!answerSet.Contains(current) && answerSet[i] == null) {
                    answerSet[i] = current;
                    i++;
                }

            }

            int n = answerSet.Length;
            // while (n > 1) {
            //     n--;
            //     int k = Random.Shared.Next(n + 1);
            //     
            //     // var value = answerSet[k];
            //     // answerSet[k] = answerSet[n];
            //     // answerSet[n] = value;
            //
            //     (answerSet[k], answerSet[n]) = (answerSet[n], answerSet[k]);
            // }

            Console.WriteLine($"What is {topic}");
            
            Console.WriteLine($"A: {answerSet[0]}\nB: {answerSet[1]}\nC: {answerSet[2]}\nD: {answerSet[3]}");
            Console.ReadLine();

            questionIndex++;
        }
    }

    public void Flashcards() {
        
    }
}