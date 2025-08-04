// Import necessary system libraries for collections, LINQ operations, and basic functionality
using System;
using System.Collections.Generic;
using System.Linq;

namespace Comp1551_ApplicationDev_CW
{
    /// <summary>
    /// Abstract base class for all question types in the geography quiz
    /// This class demonstrates key OOP principles:
    /// - ABSTRACTION: Defines common interface that all questions must implement
    /// - ENCAPSULATION: Private fields with controlled public access
    /// - INHERITANCE: Base class that other question types inherit from
    /// </summary>
    public abstract class Question
    {
        // Private fields - demonstrates ENCAPSULATION principle
        // These fields can only be accessed through public properties
        private string _questionText;  // Stores the actual question text
        private string _category;      // Stores the question category (e.g., "Geography")

        /// <summary>
        /// Public property for accessing question text with validation
        /// Demonstrates ENCAPSULATION - controlled access to private field
        /// </summary>
        public string QuestionText
        {
            get => _questionText;  // Return the private field value
            set => _questionText = value ?? throw new ArgumentNullException(nameof(value));  // Validate input - prevent null values
        }

        /// <summary>
        /// Public property for accessing question category with default value
        /// Demonstrates ENCAPSULATION with automatic default assignment
        /// </summary>
        public string Category
        {
            get => _category;  // Return the private field value
            set => _category = value ?? "General";  // Set value or use default if null
        }

        /// <summary>
        /// Protected constructor - can only be called by derived classes
        /// This enforces that Question objects can only be created through specific question types
        /// </summary>
        /// <param name="questionText">The text of the question to display</param>
        /// <param name="category">The category of the question (defaults to "Geography")</param>
        protected Question(string questionText, string category = "Geography")
        {
            QuestionText = questionText;  // Use property to ensure validation
            Category = category;          // Use property to ensure default handling
        }

        /// <summary>
        /// Abstract method to check if the provided answer is correct
        /// ABSTRACTION: Must be implemented by each derived class with their own logic
        /// POLYMORPHISM: Each question type will have different implementation
        /// </summary>
        /// <param name="userAnswer">The answer provided by the user</param>
        /// <returns>True if the answer is correct, false otherwise</returns>
        public abstract bool IsCorrect(string userAnswer);

        /// <summary>
        /// Abstract method to get the correct answer(s) for display purposes
        /// ABSTRACTION: Each question type formats their answer differently
        /// Used when showing results to the user
        /// </summary>
        /// <returns>String representation of the correct answer(s)</returns>
        public abstract string GetCorrectAnswer();

        /// <summary>
        /// Abstract method to get the question type for UI purposes
        /// ABSTRACTION: Each derived class returns its specific type
        /// Used for displaying question type in the interface
        /// </summary>
        /// <returns>String describing the question type (e.g., "Multiple Choice")</returns>
        public abstract string GetQuestionType();
    }

    /// <summary>
    /// Multiple choice question implementation - demonstrates INHERITANCE
    /// Inherits from Question base class and implements specific behavior for multiple choice questions
    /// This class handles questions with exactly 4 options where only one is correct
    /// </summary>
    public class MultipleChoiceQuestion : Question
    {
        // Private fields for storing multiple choice specific data
        private List<string> _options;        // Stores the 4 answer options (A, B, C, D)
        private int _correctOptionIndex;      // Stores which option is correct (0-3)

        /// <summary>
        /// Public property to access the answer options
        /// ENCAPSULATION: Returns a copy of the list to prevent external modification
        /// This protects the internal data while allowing read access
        /// </summary>
        public List<string> Options
        {
            get => new List<string>(_options); // Return copy to maintain encapsulation - prevents external changes
            private set => _options = value ?? throw new ArgumentNullException(nameof(value)); // Private setter with validation
        }

        /// <summary>
        /// Public property to access the correct option index
        /// ENCAPSULATION: Validates that the index is within valid range (0-3)
        /// </summary>
        public int CorrectOptionIndex
        {
            get => _correctOptionIndex;  // Return the index of correct answer
            private set
            {
                // Validation: ensure the index is valid for the options list
                if (value < 0 || value >= _options.Count)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _correctOptionIndex = value;
            }
        }

        /// <summary>
        /// Constructor for creating a multiple choice question
        /// Calls base class constructor and validates multiple choice specific requirements
        /// </summary>
        /// <param name="questionText">The question to ask the user</param>
        /// <param name="options">List of exactly 4 answer options</param>
        /// <param name="correctOptionIndex">Index (0-3) of the correct option</param>
        /// <param name="category">Question category (defaults to "Geography")</param>
        public MultipleChoiceQuestion(string questionText, List<string> options, int correctOptionIndex, string category = "Geography")
            : base(questionText, category)  // Call parent constructor first
        {
            // Validation: Multiple choice questions must have exactly 4 options
            if (options == null || options.Count != 4)
                throw new ArgumentException("Must provide exactly 4 options");

            // Set properties using validated setters
            Options = options;
            CorrectOptionIndex = correctOptionIndex;
        }

        /// <summary>
        /// POLYMORPHISM: Override abstract method to implement multiple choice answer checking
        /// Checks if user's selected option index matches the correct answer
        /// </summary>
        /// <param name="userAnswer">String representation of selected option index</param>
        /// <returns>True if user selected the correct option, false otherwise</returns>
        public override bool IsCorrect(string userAnswer)
        {
            // Try to parse user's answer as an integer (option index)
            if (int.TryParse(userAnswer, out int selectedIndex))
            {
                // Compare selected index with correct answer index
                return selectedIndex == _correctOptionIndex;
            }
            // If parsing fails, answer is incorrect
            return false;
        }

        /// <summary>
        /// POLYMORPHISM: Override abstract method to format correct answer for display
        /// Returns the correct answer in "number. text" format for user feedback
        /// </summary>
        /// <returns>Formatted string showing correct option</returns>
        public override string GetCorrectAnswer()
        {
            // Format as "3. Paris" (1-based numbering for user display)
            return $"{_correctOptionIndex + 1}. {_options[_correctOptionIndex]}";
        }

        /// <summary>
        /// POLYMORPHISM: Override abstract method to identify question type
        /// Used by UI to determine how to display the question
        /// </summary>
        /// <returns>String identifying this as a multiple choice question</returns>
        public override string GetQuestionType()
        {
            return "Multiple Choice";
        }
    }

    /// <summary>
    /// Open-ended question implementation - demonstrates INHERITANCE and flexible answer matching
    /// Inherits from Question base class and implements text-based answer checking
    /// Supports multiple correct answers and handles variations in user input (e.g., "UK" vs "United Kingdom")
    /// </summary>
    public class OpenEndedQuestion : Question
    {
        // Private field to store all acceptable correct answers
        private List<string> _correctAnswers;  // Can store multiple variations like ["London", "london", "LONDON"]

        /// <summary>
        /// Public property to access correct answers
        /// ENCAPSULATION: Returns copy to prevent external modification of internal list
        /// </summary>
        public List<string> CorrectAnswers
        {
            get => new List<string>(_correctAnswers); // Return defensive copy
            private set => _correctAnswers = value ?? throw new ArgumentNullException(nameof(value)); // Validate not null
        }

        /// <summary>
        /// Constructor for creating an open-ended question
        /// Allows multiple acceptable answers for flexibility (e.g., "UK", "United Kingdom", "Britain")
        /// </summary>
        /// <param name="questionText">The question to ask the user</param>
        /// <param name="correctAnswers">List of all acceptable correct answers</param>
        /// <param name="category">Question category (defaults to "Geography")</param>
        public OpenEndedQuestion(string questionText, List<string> correctAnswers, string category = "Geography")
            : base(questionText, category)  // Call parent constructor
        {
            // Validation: Must have at least one correct answer
            if (correctAnswers == null || correctAnswers.Count == 0)
                throw new ArgumentException("Must provide at least one correct answer");

            CorrectAnswers = correctAnswers;  // Use property for validation
        }

        /// <summary>
        /// POLYMORPHISM: Override abstract method to implement text-based answer checking
        /// Uses flexible matching to handle variations in user input
        /// Compares normalized versions of answers to be more forgiving
        /// </summary>
        /// <param name="userAnswer">Text answer provided by the user</param>
        /// <returns>True if user answer matches any acceptable answer, false otherwise</returns>
        public override bool IsCorrect(string userAnswer)
        {
            // Check for empty/null input
            if (string.IsNullOrWhiteSpace(userAnswer))
                return false;

            // Normalize user's answer (remove punctuation, standardize case, etc.)
            string normalizedUserAnswer = NormalizeAnswer(userAnswer);

            // Check if normalized user answer matches any of the normalized correct answers
            // LINQ Any() method checks if any element in the collection matches the condition
            return _correctAnswers.Any(answer =>
                NormalizeAnswer(answer).Equals(normalizedUserAnswer, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Private helper method to normalize answers for flexible comparison
        /// Removes common punctuation and formatting differences
        /// Makes comparison case-insensitive and handles common variations
        /// </summary>
        /// <param name="answer">Raw answer string to normalize</param>
        /// <returns>Normalized string for comparison</returns>
        private string NormalizeAnswer(string answer)
        {
            return answer.Trim()                    // Remove leading/trailing spaces
                        .Replace(".", "")           // Remove periods
                        .Replace(",", "")           // Remove commas
                        .Replace("'", "")           // Remove apostrophes
                        .Replace("-", " ")          // Convert hyphens to spaces
                        .ToLowerInvariant();        // Convert to lowercase for case-insensitive comparison
        }

        /// <summary>
        /// POLYMORPHISM: Override abstract method to format all correct answers for display
        /// Shows all acceptable answers separated by " / " for user feedback
        /// </summary>
        /// <returns>String showing all acceptable answers</returns>
        public override string GetCorrectAnswer()
        {
            // Join all correct answers with " / " separator (e.g., "Russia / Russian Federation")
            return string.Join(" / ", _correctAnswers);
        }

        /// <summary>
        /// POLYMORPHISM: Override abstract method to identify question type
        /// Used by UI to determine how to display the question (text input box)
        /// </summary>
        /// <returns>String identifying this as an open-ended question</returns>
        public override string GetQuestionType()
        {
            return "Open-Ended";
        }
    }

    /// <summary>
    /// True/False question implementation - demonstrates INHERITANCE and boolean logic
    /// Inherits from Question base class and implements binary choice questions
    /// Handles statements that are either true or false
    /// </summary>
    public class TrueFalseQuestion : Question
    {
        // Private field to store the correct boolean answer
        private bool _correctAnswer;  // true = statement is correct, false = statement is incorrect

        /// <summary>
        /// Public property to access the correct boolean answer
        /// ENCAPSULATION: Controlled access to private field
        /// </summary>
        public bool CorrectAnswer
        {
            get => _correctAnswer;              // Return the boolean value
            private set => _correctAnswer = value;  // Simple assignment (boolean validation not needed)
        }

        /// <summary>
        /// Constructor for creating a true/false question
        /// Takes a statement and whether that statement is true or false
        /// </summary>
        /// <param name="questionText">The statement to evaluate (e.g., "The Earth is flat")</param>
        /// <param name="correctAnswer">Whether the statement is true (true) or false (false)</param>
        /// <param name="category">Question category (defaults to "Geography")</param>
        public TrueFalseQuestion(string questionText, bool correctAnswer, string category = "Geography")
            : base(questionText, category)  // Call parent constructor
        {
            CorrectAnswer = correctAnswer;  // Set the correct answer using property
        }

        /// <summary>
        /// POLYMORPHISM: Override abstract method to implement true/false answer checking
        /// Handles multiple input formats: boolean values, text representations, and common abbreviations
        /// Very flexible to handle different ways users might input true/false
        /// </summary>
        /// <param name="userAnswer">User's answer in various possible formats</param>
        /// <returns>True if user's answer matches the correct boolean value</returns>
        public override bool IsCorrect(string userAnswer)
        {
            // First try to parse as a direct boolean value
            if (bool.TryParse(userAnswer, out bool boolAnswer))
            {
                return boolAnswer == _correctAnswer;
            }

            // Handle string representations - normalize input for flexible matching
            string normalized = userAnswer.Trim().ToLowerInvariant();

            // Check for "true" variations
            if (normalized == "true" || normalized == "t" || normalized == "yes" || normalized == "y")
                return _correctAnswer == true;  // User said true, check if correct answer is true

            // Check for "false" variations
            if (normalized == "false" || normalized == "f" || normalized == "no" || normalized == "n")
                return _correctAnswer == false; // User said false, check if correct answer is false

            // If input doesn't match any recognized format, it's incorrect
            return false;
        }

        /// <summary>
        /// POLYMORPHISM: Override abstract method to format correct answer for display
        /// Returns simple "True" or "False" string for user feedback
        /// </summary>
        /// <returns>String representation of correct boolean answer</returns>
        public override string GetCorrectAnswer()
        {
            // Use ternary operator to convert boolean to string
            return _correctAnswer ? "True" : "False";
        }

        /// <summary>
        /// POLYMORPHISM: Override abstract method to identify question type
        /// Used by UI to determine how to display the question (True/False buttons)
        /// </summary>
        /// <returns>String identifying this as a true/false question</returns>
        public override string GetQuestionType()
        {
            return "True/False";
        }
    }
} // End of namespace
