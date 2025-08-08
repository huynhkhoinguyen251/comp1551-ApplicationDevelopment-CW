using System;
using System.Collections.Generic;
using System.Linq;

namespace Comp1551_ApplicationDev_CW
{
    // Abstract base class - defines structure for all question types
    public abstract class Question
    {
        // Private fields - encapsulation
        private string _questionText;
        private string _category;

        // Property with validation - prevents null values
        public string QuestionText
        {
            get => _questionText;
            set => _questionText = value ?? throw new ArgumentNullException(nameof(value));
        }

        // Property with default value - sets "General" if null
        public string Category
        {
            get => _category;
            set => _category = value ?? "General";
        }

        // Protected constructor - only derived classes can use
        protected Question(string questionText, string category = "Geography")
        {
            QuestionText = questionText;
            Category = category;
        }

        // Abstract method - each question type implements differently
        public abstract bool IsCorrect(string userAnswer);

        // Abstract method - returns correct answer for display
        public abstract string GetCorrectAnswer();

        // Abstract method - returns question type name
        public abstract string GetQuestionType();
    }

    // Multiple choice question - inherits from Question
    public class MultipleChoiceQuestion : Question
    {
        // Private fields - stores options and correct answer index
        private List<string> _options;
        private int _correctOptionIndex;

        // Property returns copy - prevents external modification
        public List<string> Options
        {
            get => new List<string>(_options);
            private set => _options = value ?? throw new ArgumentNullException(nameof(value));
        }

        // Property with validation - ensures valid index range
        public int CorrectOptionIndex
        {
            get => _correctOptionIndex;
            private set
            {
                if (value < 0 || value >= _options.Count)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _correctOptionIndex = value;
            }
        }

        // Constructor - creates multiple choice question with 4 options
        public MultipleChoiceQuestion(string questionText, List<string> options, int correctOptionIndex, string category = "Geography")
            : base(questionText, category)
        {
            if (options == null || options.Count != 4)
                throw new ArgumentException("Must provide exactly 4 options");
            
            Options = options;
            CorrectOptionIndex = correctOptionIndex;
        }

        // Override method - checks if selected index matches correct answer
        public override bool IsCorrect(string userAnswer)
        {
            if (int.TryParse(userAnswer, out int selectedIndex))
            {
                return selectedIndex == _correctOptionIndex;
            }
            return false;
        }

        // Override method - returns formatted correct answer
        public override string GetCorrectAnswer()
        {
            return $"{_correctOptionIndex + 1}. {_options[_correctOptionIndex]}";
        }

        // Override method - returns question type identifier
        public override string GetQuestionType()
        {
            return "Multiple Choice";
        }
    }

    // Open-ended question - accepts text answers
    public class OpenEndedQuestion : Question
    {
        // Private field - stores multiple acceptable answers
        private List<string> _correctAnswers;

        // Property returns copy - prevents external modification
        public List<string> CorrectAnswers
        {
            get => new List<string>(_correctAnswers);
            private set => _correctAnswers = value ?? throw new ArgumentNullException(nameof(value));
        }

        // Constructor - creates open-ended question with multiple correct answers
        public OpenEndedQuestion(string questionText, List<string> correctAnswers, string category = "Geography")
            : base(questionText, category)
        {
            if (correctAnswers == null || correctAnswers.Count == 0)
                throw new ArgumentException("Must provide at least one correct answer");
            
            CorrectAnswers = correctAnswers;
        }

        // Override method - checks if user answer matches any correct answer
        public override bool IsCorrect(string userAnswer)
        {
            if (string.IsNullOrWhiteSpace(userAnswer))
                return false;

            string normalizedUserAnswer = NormalizeAnswer(userAnswer);
            
            return _correctAnswers.Any(answer => 
                NormalizeAnswer(answer).Equals(normalizedUserAnswer, StringComparison.OrdinalIgnoreCase));
        }

        // Helper method - normalizes text for flexible comparison
        private string NormalizeAnswer(string answer)
        {
            return answer.Trim()
                        .Replace(".", "")
                        .Replace(",", "")
                        .Replace("'", "")
                        .Replace("-", " ")
                        .ToLowerInvariant();
        }

        // Override method - returns all correct answers joined with "/"
        public override string GetCorrectAnswer()
        {
            return string.Join(" / ", _correctAnswers);
        }

        // Override method - returns question type identifier
        public override string GetQuestionType()
        {
            return "Open-Ended";
        }
    }

    // True/False question - boolean logic questions
    public class TrueFalseQuestion : Question
    {
        // Private field - stores correct boolean answer
        private bool _correctAnswer;

        // Property - gets/sets correct answer
        public bool CorrectAnswer
        {
            get => _correctAnswer;
            private set => _correctAnswer = value;
        }

        // Constructor - creates true/false question
        public TrueFalseQuestion(string questionText, bool correctAnswer, string category = "Geography")
            : base(questionText, category)
        {
            CorrectAnswer = correctAnswer;
        }

        // Override method - checks if user answer matches correct boolean value
        public override bool IsCorrect(string userAnswer)
        {
            if (bool.TryParse(userAnswer, out bool boolAnswer))
            {
                return boolAnswer == _correctAnswer;
            }
            
            // Handle text variations - "true", "yes", "false", "no", etc.
            string normalized = userAnswer.Trim().ToLowerInvariant();
            if (normalized == "true" || normalized == "t" || normalized == "yes" || normalized == "y")
                return _correctAnswer == true;
            if (normalized == "false" || normalized == "f" || normalized == "no" || normalized == "n")
                return _correctAnswer == false;
                
            return false;
        }

        // Override method - returns "True" or "False"
        public override string GetCorrectAnswer()
        {
            return _correctAnswer ? "True" : "False";
        }

        // Override method - returns question type identifier
        public override string GetQuestionType()
        {
            return "True/False";
        }
    }
}
