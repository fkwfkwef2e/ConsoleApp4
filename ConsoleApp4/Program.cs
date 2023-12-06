using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

public class User
{
    public string Name { get; set; }
    public int CharactersPerMinute { get; set; }
    public int CharactersPerSecond { get; set; }
}

public static class Leaderboard
{
    private static List<User> users = new List<User>();

    public static void AddUser(User user)
    {
        users.Add(user);
    }

    public static void ShowLeaderboard()
    {
        Console.WriteLine("Leaderboard:");
        foreach (var user in users)
        {
            Console.WriteLine($"{user.Name} - {user.CharactersPerMinute} characters per minute, {user.CharactersPerSecond} characters per second");
        }
    }

    public static void SaveLeaderboard()
    {
        string json = JsonConvert.SerializeObject(users);
        File.WriteAllText("leaderboard.json", json);
    }

    public static void LoadLeaderboard()
    {
        if (File.Exists("leaderboard.json"))
        {
            string json = File.ReadAllText("leaderboard.json");
            users = JsonConvert.DeserializeObject<List<User>>(json);
        }
    }
}

public class TypingTest
{
    private string textToType;
    private Stopwatch stopwatch = new Stopwatch();

    public TypingTest(string text)
    {
        textToType = text;
    }

    public void StartTest()
    {
        Console.Write("Enter your name: ");
        string name = Console.ReadLine();

        stopwatch.Start();

        Console.WriteLine($"Type the following text: {textToType}");

        StringBuilder typedText = new StringBuilder();
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
            if (key.Key != ConsoleKey.Enter)
            {
                typedText.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        } while (stopwatch.Elapsed < TimeSpan.FromMinutes(1) && typedText.ToString() != textToType);

        stopwatch.Stop();

        int charactersTyped = typedText.Length;
        double minutes = stopwatch.Elapsed.TotalMinutes;
        double seconds = stopwatch.Elapsed.TotalSeconds;

        User user = new User
        {
            Name = name,
            CharactersPerMinute = (int)(charactersTyped / minutes),
            CharactersPerSecond = (int)(charactersTyped / seconds)
        };

        Leaderboard.AddUser(user);
        Leaderboard.ShowLeaderboard();
        Leaderboard.SaveLeaderboard();
    }
}

class Program
{
    static void Main(string[] args)
    {
        Leaderboard.LoadLeaderboard();

        string text = "This is a typing test. You should type this exact sentence as fast as you can.";

        TypingTest typingTest = new TypingTest(text);
        typingTest.StartTest();

        // Allow the user to take the test again without closing the program
        while (true)
        {
            Console.WriteLine("Do you want to take the test again? (yes/no)");
            string answer = Console.ReadLine();
            if (answer.ToLower() != "yes")
            {
                break;
            }
            typingTest.StartTest();
        }
    }
}