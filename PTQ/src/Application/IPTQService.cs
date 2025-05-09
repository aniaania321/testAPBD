using Models;

namespace Application;

public interface IPTQService
{
    public List<QuizDTO> GetAllQuizzes();
    public QuizDTO GetQuizById(int id);
    public void AddQuiz(QuizDTO quiz);
    
}