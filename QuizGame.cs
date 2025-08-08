using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Comp1551_ApplicationDev_CW
{
    // Main game logic class - manages quiz state and questions
    public class QuizGame
    {
        // Private fields - encapsulation principle
        private List<Question> _questions;        // Variable-size data structure - stores all questions
        private Stopwatch _gameTimer;            // Tracks time spent on quiz
        private int _currentQuestionIndex;       // Current question position (0-based)
        private List<bool> _userAnswers;         // Stores correctness of each answer
        private List<string> _userResponses;     // Stores actual user responses

        // Read-only properties - controlled access to internal state
        public int TotalQuestions => _questions.Count;                    // Returns number of questions
        public int CurrentQuestionIndex => _currentQuestionIndex;         // Returns current position
        public bool IsGameInProgress { get; private set; }               // Game active status
        public TimeSpan ElapsedTime => _gameTimer?.Elapsed ?? TimeSpan.Zero;  // Time elapsed

        // Constructor - initializes all data structures
        public QuizGame()
        {
            _questions = new List<Question>();      // Initialize empty question list
            _gameTimer = new Stopwatch();          // Initialize timer
            _userAnswers = new List<bool>();       // Initialize answer tracking
            _userResponses = new List<string>();   // Initialize response tracking
            _currentQuestionIndex = 0;             // Start at first question
            IsGameInProgress = false;              // Game not started
        }

        // Adds question to quiz - polymorphism (accepts any Question type)
        public void AddQuestion(Question question)
        {
            if (question == null)                  // Validation - prevent null
                throw new ArgumentNullException(nameof(question));

            _questions.Add(question);              // Add to list
        }

        // Removes question at specified index - returns success status
        public bool RemoveQuestion(int index)
        {
            if (index < 0 || index >= _questions.Count)  // Bounds checking
                return false;

            _questions.RemoveAt(index);            // Remove from list
            return true;                           // Success
        }

        // Updates question at index - polymorphism (accepts any Question type)
        public bool UpdateQuestion(int index, Question newQuestion)
        {
            if (index < 0 || index >= _questions.Count || newQuestion == null)  // Validation
                return false;

            _questions[index] = newQuestion;       // Replace question
            return true;                           // Success
        }

        public Question GetQuestion(int index)
        {
            if (index < 0 || index >= _questions.Count)
                return null;

            return _questions[index];
        }

        public List<Question> GetAllQuestions()
        {
            return new List<Question>(_questions);
        }
        
        public Question GetCurrentQuestion()
        {
            if (!IsGameInProgress || _currentQuestionIndex >= _questions.Count)
                return null;

            return _questions[_currentQuestionIndex];
        }

        public bool StartGame()
        {
            if (_questions.Count == 0)
                return false;

            _currentQuestionIndex = 0;
            _userAnswers.Clear();
            _userResponses.Clear();
            _gameTimer.Reset();
            _gameTimer.Start();
            IsGameInProgress = true;

            return true;
        }

        public bool SubmitAnswer(string userAnswer)
        {
            if (!IsGameInProgress || _currentQuestionIndex >= _questions.Count)
                return false;

            Question currentQuestion = _questions[_currentQuestionIndex];
            bool isCorrect = currentQuestion.IsCorrect(userAnswer);

            _userAnswers.Add(isCorrect);
            _userResponses.Add(userAnswer ?? "");

            _currentQuestionIndex++;

            if (_currentQuestionIndex >= _questions.Count)
            {
                _gameTimer.Stop();
                IsGameInProgress = false;
            }

            return true;
        }

        public bool HasNextQuestion()
        {
            return IsGameInProgress && _currentQuestionIndex < _questions.Count;
        }

        public GameResult GetGameResult()
        {
            if (IsGameInProgress)
                return null;

            int correctAnswers = 0;
            foreach (bool answer in _userAnswers)
            {
                if (answer) correctAnswers++;
            }

            return new GameResult
            {
                TotalQuestions = _questions.Count,
                CorrectAnswers = correctAnswers,
                TimeTaken = _gameTimer.Elapsed,
                Questions = new List<Question>(_questions),
                UserAnswers = new List<bool>(_userAnswers),
                UserResponses = new List<string>(_userResponses)
            };
        }

        public void ResetGame()
        {
            _gameTimer.Reset();
            _currentQuestionIndex = 0;
            _userAnswers.Clear();
            _userResponses.Clear();
            IsGameInProgress = false;
        }

        public void ClearAllQuestions()
        {
            _questions.Clear();
            ResetGame();
        }
    }

    public class GameResult
    {
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int IncorrectAnswers => TotalQuestions - CorrectAnswers;
        public double ScorePercentage => TotalQuestions > 0 ? (double)CorrectAnswers / TotalQuestions * 100 : 0;
        public TimeSpan TimeTaken { get; set; }
        public double TimeTakenMinutes => TimeTaken.TotalMinutes;
        public List<Question> Questions { get; set; } = new List<Question>();
        public List<bool> UserAnswers { get; set; } = new List<bool>();
        public List<string> UserResponses { get; set; } = new List<string>();
    } 
}
