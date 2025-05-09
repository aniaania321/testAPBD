using Application;
using Models;
using Repositories;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddTransient<IPTQRepository>(_ => new PTQRepository(connectionString));
builder.Services.AddTransient<IPTQService, PTQService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/quizzes", (IPTQService service) =>
{
    try
    {
        return Results.Ok(service.GetAllQuizzes());
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/api/quizzes/{id}", (int id, IPTQService service) =>
{
    try
    {
        var quiz = service.GetQuizById(id);
        if (quiz == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(quiz);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPost("/api/quizzes", (QuizDTO quiz, IPTQService service) =>
{
    try
    {
        service.AddQuiz(quiz);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();