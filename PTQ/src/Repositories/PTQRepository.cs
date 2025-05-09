using Microsoft.Data.SqlClient;
using Models;

namespace Repositories;

public class PTQRepository : IPTQRepository
{
    private string _connectionString;

    public PTQRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<QuizDTO> GetAllQuizzes()
    {
        var quizzes = new List<QuizDTO>();

        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            var cmd = new SqlCommand("SELECT Id, Name FROM Quiz", conn);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                quizzes.Add(new QuizDTO
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"].ToString()
                });
            }
        }

        return quizzes;
    }

    public QuizDTO GetQuizById(int id)
    {
        QuizDTO quiz = null;

        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            var cmd = new SqlCommand(@"
                SELECT q.Id, q.Name, q.PathFile, t.Name as PotatoTeacherName 
                FROM Quiz q 
                JOIN PotatoTeacher t ON q.PotatoTeacherId = t.Id 
                WHERE q.Id = @Id", conn);

            cmd.Parameters.AddWithValue("@Id", id);

            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                quiz = new QuizDTO
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"].ToString(),
                    PathFile = reader["PathFile"].ToString(),
                    PotatoTeacherName = reader["PotatoTeacherName"].ToString()
                };
            }
        }

        return quiz;
    }

    public void AddQuiz(QuizDTO quiz)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            var transaction = conn.BeginTransaction();

            try
            {
                var checkCmd = new SqlCommand("SELECT Id FROM PotatoTeacher WHERE Name = @Name", conn, transaction);
                checkCmd.Parameters.AddWithValue("@Name", quiz.PotatoTeacherName);
                object result = checkCmd.ExecuteScalar();

                int teacherId;
                if (result != null)
                {
                    teacherId = (int)result;
                }
                else
                {
                    var insertTeacher = new SqlCommand("INSERT INTO PotatoTeacher (Name) OUTPUT INSERTED.Id VALUES (@Name)", conn, transaction);
                    insertTeacher.Parameters.AddWithValue("@Name", quiz.PotatoTeacherName);
                    teacherId = (int)insertTeacher.ExecuteScalar();
                }

                var insertQuiz = new SqlCommand(
                    "INSERT INTO Quiz (Name, PotatoTeacherId, PathFile) VALUES (@Name, @TeacherId, @PathFile)", conn, transaction);

                insertQuiz.Parameters.AddWithValue("@Name", quiz.Name);
                insertQuiz.Parameters.AddWithValue("@TeacherId", teacherId);
                insertQuiz.Parameters.AddWithValue("@PathFile", quiz.PathFile);

                insertQuiz.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
