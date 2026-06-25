namespace CybersecurityAwarenessBot.Models;

public class QuizAnswerResult
{
    public bool IsCorrect { get; set; }
    public bool IsFinished { get; set; }
    public string Feedback { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
}
