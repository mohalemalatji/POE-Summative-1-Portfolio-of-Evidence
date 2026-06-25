using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services;

public class QuizService
{
    private readonly List<QuizQuestion> _questions = new()
    {
        new QuizQuestion
        {
            QuestionText = "What is phishing?",
            Options = new List<string> { "A fake message that tries to steal information", "A type of firewall", "A strong password method", "A backup system" },
            CorrectAnswerIndex = 0,
            Explanation = "Phishing tricks users into giving away personal details through fake emails, messages, or websites."
        },
        new QuizQuestion
        {
            QuestionText = "True or False: You should use the same password for all accounts.",
            Options = new List<string> { "True", "False" },
            CorrectAnswerIndex = 1,
            Explanation = "Reusing passwords is risky because one leaked password can expose many accounts."
        },
        new QuizQuestion
        {
            QuestionText = "Which option is safest for a password?",
            Options = new List<string> { "12345678", "Your birth date", "A long unique passphrase", "Your pet's name" },
            CorrectAnswerIndex = 2,
            Explanation = "A long unique passphrase is harder to guess and safer than personal details."
        },
        new QuizQuestion
        {
            QuestionText = "What does 2FA add to an account?",
            Options = new List<string> { "A second verification step", "A weaker login", "A public password", "A browser theme" },
            CorrectAnswerIndex = 0,
            Explanation = "Two-factor authentication adds another step after the password."
        },
        new QuizQuestion
        {
            QuestionText = "True or False: HTTPS helps protect information sent between you and a website.",
            Options = new List<string> { "True", "False" },
            CorrectAnswerIndex = 0,
            Explanation = "HTTPS encrypts traffic between your browser and the website."
        },
        new QuizQuestion
        {
            QuestionText = "What is social engineering?",
            Options = new List<string> { "Manipulating people to reveal information", "Installing updates", "Backing up files", "Changing screen brightness" },
            CorrectAnswerIndex = 0,
            Explanation = "Social engineering targets people by using pressure, trust, or deception."
        },
        new QuizQuestion
        {
            QuestionText = "Which action is safest when you receive an unexpected attachment?",
            Options = new List<string> { "Open it quickly", "Forward it", "Check the sender first", "Disable antivirus" },
            CorrectAnswerIndex = 2,
            Explanation = "Unexpected attachments can contain malware, so check the sender and context first."
        },
        new QuizQuestion
        {
            QuestionText = "True or False: Public Wi-Fi is always safe for online banking.",
            Options = new List<string> { "True", "False" },
            CorrectAnswerIndex = 1,
            Explanation = "Public Wi-Fi can be risky, especially for banking or private accounts."
        },
        new QuizQuestion
        {
            QuestionText = "What is malware?",
            Options = new List<string> { "Helpful software", "Harmful software", "A password manager", "A browser bookmark" },
            CorrectAnswerIndex = 1,
            Explanation = "Malware is software designed to harm, spy, steal, or damage systems."
        },
        new QuizQuestion
        {
            QuestionText = "What should you do before clicking a suspicious link?",
            Options = new List<string> { "Click it first", "Check where it leads", "Share it with friends", "Enter your password" },
            CorrectAnswerIndex = 1,
            Explanation = "Checking the link helps you avoid fake or dangerous websites."
        },
        new QuizQuestion
        {
            QuestionText = "True or False: Software updates can fix security weaknesses.",
            Options = new List<string> { "True", "False" },
            CorrectAnswerIndex = 0,
            Explanation = "Updates often patch known vulnerabilities."
        },
        new QuizQuestion
        {
            QuestionText = "Which is a common sign of a scam?",
            Options = new List<string> { "No pressure", "Clear official contact", "Urgent demand for money or OTPs", "Normal spelling and branding" },
            CorrectAnswerIndex = 2,
            Explanation = "Scams often pressure people to act fast or share sensitive codes."
        }
    };

    private int _currentIndex;
    private int _score;

    public bool IsActive { get; private set; }
    public int Score => _score;
    public int TotalQuestions => _questions.Count;
    public QuizQuestion? CurrentQuestion => IsActive && _currentIndex < _questions.Count ? _questions[_currentIndex] : null;


    public QuizQuestion Start()
    {
        _currentIndex = 0;
        _score = 0;
        IsActive = true;
        return _questions[_currentIndex];
    }


    public QuizAnswerResult SubmitAnswer(int selectedIndex)
    {
        QuizQuestion question = _questions[_currentIndex];
        bool isCorrect = selectedIndex == question.CorrectAnswerIndex;

        if (isCorrect)
        {
            _score++;
        }

        string answerText = isCorrect ? "Correct." : "Incorrect.";
        string feedback = $"{answerText} {question.Explanation}";

        _currentIndex++;
        bool isFinished = _currentIndex >= _questions.Count;

        if (isFinished)
        {
            IsActive = false;
            feedback += $" Final score: {_score}/{_questions.Count}. {GetPerformanceMessage()}";
        }

        return new QuizAnswerResult
        {
            IsCorrect = isCorrect,
            IsFinished = isFinished,
            Feedback = feedback,
            Score = _score,
            TotalQuestions = _questions.Count
        };
    }


    private string GetPerformanceMessage()
    {
        double percentage = (double)_score / _questions.Count;

        if (percentage >= 0.8)
        {
            return "Excellent work. You have strong cybersecurity awareness.";
        }

        if (percentage >= 0.5)
        {
            return "Good effort. Review the explanations to improve even more.";
        }

        return "Keep practising. Cybersecurity gets easier the more you learn.";
    }
}
