using Models;

namespace Repositories;

public interface IPTQRepository
{
    public List<QuizDTO> GetAllQuizzes();
    public QuizDTO GetQuizById(int id);
    public void AddQuiz(QuizDTO quiz);


}