using System;
using System.Collections.Generic;
using System.Linq;

namespace Comp1551_ApplicationDev_CW
{
    public class QuestionFormController
    {
        public enum QuestionType
        {
            MultipleChoice,
            OpenEnded,
            TrueFalse
        }

        public class QuestionData
        {
            public string QuestionText { get; set; } = "";
            public QuestionType Type { get; set; }
            public List<string> Options { get; set; } = new List<string>();
            public int CorrectOptionIndex { get; set; }
            public List<string> CorrectAnswers { get; set; } = new List<string>();
            public bool TrueFalseAnswer { get; set; }
        }

        public List<string> GetQuestionTypes()
        {
            return new List<string> { "Multiple Choice", "Open-Ended", "True/False" };
        }

        public QuestionType ParseQuestionType(string typeString)
        {
            return typeString switch
            {
                "Multiple Choice" => QuestionType.MultipleChoice,
                "Open-Ended" => QuestionType.OpenEnded,
                "True/False" => QuestionType.TrueFalse,
                _ => throw new ArgumentException("Invalid question type")
            };
        }

        public QuestionData ExtractQuestionData(Question question)
        {
            var data = new QuestionData
            {
                QuestionText = question.QuestionText
            };

            switch (question)
            {
                case MultipleChoiceQuestion mcq:
                    data.Type = QuestionType.MultipleChoice;
                    data.Options = mcq.Options;
                    data.CorrectOptionIndex = mcq.CorrectOptionIndex;
                    break;
                    
                case OpenEndedQuestion oeq:
                    data.Type = QuestionType.OpenEnded;
                    data.CorrectAnswers = oeq.CorrectAnswers;
                    break;
                    
                case TrueFalseQuestion tfq:
                    data.Type = QuestionType.TrueFalse;
                    data.TrueFalseAnswer = tfq.CorrectAnswer;
                    break;
            }

            return data;
        }

        public Question CreateQuestion(QuestionData data)
        {
            ValidateQuestionData(data);

            return data.Type switch
            {
                QuestionType.MultipleChoice => CreateMultipleChoiceQuestion(data),
                QuestionType.OpenEnded => CreateOpenEndedQuestion(data),
                QuestionType.TrueFalse => CreateTrueFalseQuestion(data),
                _ => throw new ArgumentException("Invalid question type")
            };
        }

        private void ValidateQuestionData(QuestionData data)
        {
            if (string.IsNullOrWhiteSpace(data.QuestionText))
                throw new ArgumentException("Question text cannot be empty");

            switch (data.Type)
            {
                case QuestionType.MultipleChoice:
                    ValidateMultipleChoiceData(data);
                    break;
                case QuestionType.OpenEnded:
                    ValidateOpenEndedData(data);
                    break;
            }
        }

        private void ValidateMultipleChoiceData(QuestionData data)
        {
            if (data.Options.Count != 4)
                throw new ArgumentException("Multiple choice questions must have exactly 4 options");

            if (data.Options.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("All options must be filled");

            if (data.CorrectOptionIndex < 0 || data.CorrectOptionIndex >= 4)
                throw new ArgumentException("Please select the correct answer");
        }

        private void ValidateOpenEndedData(QuestionData data)
        {
            if (data.CorrectAnswers.Count == 0 || data.CorrectAnswers.All(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Please enter at least one correct answer");
        }

        private MultipleChoiceQuestion CreateMultipleChoiceQuestion(QuestionData data)
        {
            return new MultipleChoiceQuestion(data.QuestionText, data.Options, data.CorrectOptionIndex);
        }

        private OpenEndedQuestion CreateOpenEndedQuestion(QuestionData data)
        {
            var answers = data.CorrectAnswers.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
            return new OpenEndedQuestion(data.QuestionText, answers);
        }

        private TrueFalseQuestion CreateTrueFalseQuestion(QuestionData data)
        {
            return new TrueFalseQuestion(data.QuestionText, data.TrueFalseAnswer);
        }
    }
}
