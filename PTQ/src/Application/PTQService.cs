using Models;
using Repositories;

namespace Application;

public class PTQService : IPTQService
{
    private readonly IPTQRepository _repository;

    public PTQService(IPTQRepository repository)
    {
        _repository = repository;
    }

    public List<QuizDTO> GetAllQuizzes()
    {
        return _repository.GetAllQuizzes();
    }

    public QuizDTO GetQuizById(int id)
    {
        return _repository.GetQuizById(id);
    }

    public void AddQuiz(QuizDTO quiz)
    {
        _repository.AddQuiz(quiz);
    }
}
