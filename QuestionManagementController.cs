using System;
using System.Collections.Generic;

namespace Comp1551_ApplicationDev_CW
{
    public class QuestionManagementController
    {
        private QuizGame _quizGame;

        public QuestionManagementController(QuizGame quizGame)
        {
            _quizGame = quizGame ?? throw new ArgumentNullException(nameof(quizGame));
        }

        public List<string> GetQuestionDisplayList()
        {
            var questions = _quizGame.GetAllQuestions();
            var displayList = new List<string>();
            
            for (int i = 0; i < questions.Count; i++)
            {
                displayList.Add($"{i + 1}. [{questions[i].GetQuestionType()}] {questions[i].QuestionText}");
            }
            
            return displayList;
        }

        public bool AddQuestion(Question question)
        {
            try
            {
                _quizGame.AddQuestion(question);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateQuestion(int index, Question newQuestion)
        {
            return _quizGame.UpdateQuestion(index, newQuestion);
        }

        public bool DeleteQuestion(int index)
        {
            return _quizGame.RemoveQuestion(index);
        }

        public Question GetQuestion(int index)
        {
            return _quizGame.GetQuestion(index);
        }

        public int GetQuestionCount()
        {
            return _quizGame.TotalQuestions;
        }

        public bool HasQuestions()
        {
            return _quizGame.TotalQuestions > 0;
        }
    }
}
