using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Comp1551_ApplicationDev_CW
{
    public partial class QuizPlayForm : Form
    {
        private QuizGame _quizGame;
        private Label _questionLabel = null!;
        private Panel _answerPanel = null!;
        private Button _nextButton = null!;
        private Label _progressLabel = null!;

        public QuizPlayForm(QuizGame quizGame)
        {
            _quizGame = quizGame;
            SetupUI();
            StartQuiz();
        }

        private void SetupUI()
        {
            this.Text = "🎮 Geography Quiz - Play Mode";
            this.Size = new Size(800, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 245);

            // Header panel with gradient
            Panel headerPanel = new Panel
            {
                Size = new Size(800, 80),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(25, 118, 210)
            };
            headerPanel.Paint += (s, e) => {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    new Point(0, 0), new Point(0, 80),
                    Color.FromArgb(25, 118, 210),
                    Color.FromArgb(33, 150, 243)))
                {
                    e.Graphics.FillRectangle(brush, headerPanel.ClientRectangle);
                }
            };

            // Title in header
            Label titleLabel = new Label
            {
                Text = "🌍 GEOGRAPHY QUIZ CHALLENGE 🌍",
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(200, 25)
            };

            _progressLabel = new Label
            {
                Location = new Point(50, 100),
                Size = new Size(700, 40),
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 118, 210),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Question panel with styling
            Panel questionPanel = new Panel
            {
                Location = new Point(50, 160),
                Size = new Size(700, 120),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            _questionLabel = new Label
            {
                Location = new Point(20, 20),
                Size = new Size(660, 80),
                Font = new Font("Arial", 13, FontStyle.Regular),
                ForeColor = Color.FromArgb(33, 33, 33),
                TextAlign = ContentAlignment.TopLeft,
                BackColor = Color.Transparent
            };

            _answerPanel = new Panel
            {
                Location = new Point(50, 300),
                Size = new Size(700, 220),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            _nextButton = new Button
            {
                Text = "➡️ Next Question",
                Size = new Size(150, 50),
                Location = new Point(600, 550),
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand
            };
            _nextButton.Click += NextButton_Click;

            // Add hover effect to next button
            _nextButton.MouseEnter += (s, e) => _nextButton.BackColor = Color.FromArgb(76, 175, 80);
            _nextButton.MouseLeave += (s, e) => _nextButton.BackColor = Color.FromArgb(46, 125, 50);

            // Add controls
            headerPanel.Controls.Add(titleLabel);
            questionPanel.Controls.Add(_questionLabel);

            this.Controls.AddRange(new Control[] { headerPanel, _progressLabel, questionPanel, _answerPanel, _nextButton });
        }

        private void StartQuiz()
        {
            _quizGame.StartGame();
            DisplayCurrentQuestion();
        }

        private void DisplayCurrentQuestion()
        {
            var question = _quizGame.GetCurrentQuestion();
            if (question == null)
            {
                ShowResults();
                return;
            }

            _progressLabel.Text = $"Question {_quizGame.CurrentQuestionIndex + 1} of {_quizGame.TotalQuestions}";
            _questionLabel.Text = question.QuestionText;

            _answerPanel.Controls.Clear();

            if (question is MultipleChoiceQuestion mcq)
            {
                DisplayMultipleChoice(mcq);
            }
            else if (question is OpenEndedQuestion)
            {
                DisplayOpenEnded();
            }
            else if (question is TrueFalseQuestion)
            {
                DisplayTrueFalse();
            }
        }

        private void DisplayMultipleChoice(MultipleChoiceQuestion mcq)
        {
            var options = mcq.Options;
            string[] optionLetters = { "A", "B", "C", "D" };

            for (int i = 0; i < options.Count; i++)
            {
                RadioButton rb = new RadioButton
                {
                    Text = $"{optionLetters[i]}. {options[i]}",
                    Location = new Point(30, 20 + i * 45),
                    Size = new Size(630, 40),
                    Font = new Font("Arial", 11),
                    ForeColor = Color.FromArgb(33, 33, 33),
                    Tag = i,
                    BackColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Appearance = Appearance.Normal,
                    UseVisualStyleBackColor = false,
                    Cursor = Cursors.Hand,
                    Padding = new Padding(10)
                };

                // Add hover effects
                rb.MouseEnter += (s, e) => rb.BackColor = Color.FromArgb(227, 242, 253);
                rb.MouseLeave += (s, e) => rb.BackColor = Color.White;

                // Ensure mutual exclusion by clearing other radio buttons when one is selected
                rb.CheckedChanged += (s, e) => {
                    if (rb.Checked)
                    {
                        foreach (Control control in _answerPanel.Controls)
                        {
                            if (control is RadioButton otherRb && otherRb != rb)
                            {
                                otherRb.Checked = false;
                            }
                        }
                    }
                };

                _answerPanel.Controls.Add(rb);
            }
        }

        private void DisplayOpenEnded()
        {
            Label instructionLabel = new Label
            {
                Text = "💭 Type your answer below:",
                Location = new Point(30, 20),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 118, 210),
                AutoSize = true
            };

            TextBox textBox = new TextBox
            {
                Location = new Point(30, 60),
                Size = new Size(600, 40),
                Font = new Font("Arial", 14),
                Name = "answerTextBox",
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true
            };

            Label hint = new Label
            {
                Text = "💡 Hint: Enter 1-4 words. Alternative spellings are accepted!",
                Location = new Point(30, 120),
                Font = new Font("Arial", 10, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = true
            };

            _answerPanel.Controls.AddRange(new Control[] { instructionLabel, textBox, hint });
        }

        private void DisplayTrueFalse()
        {
            Label instructionLabel = new Label
            {
                Text = "🤔 Is this statement true or false?",
                Location = new Point(30, 20),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 118, 210),
                AutoSize = true
            };

            RadioButton trueRb = new RadioButton
            {
                Text = "✅ TRUE",
                Location = new Point(100, 70),
                Size = new Size(200, 50),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 125, 50),
                Tag = "True",
                BackColor = Color.FromArgb(232, 245, 233),
                FlatStyle = FlatStyle.Flat,
                Appearance = Appearance.Button,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };

            RadioButton falseRb = new RadioButton
            {
                Text = "❌ FALSE",
                Location = new Point(400, 70),
                Size = new Size(200, 50),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(183, 28, 28),
                Tag = "False",
                BackColor = Color.FromArgb(255, 235, 238),
                FlatStyle = FlatStyle.Flat,
                Appearance = Appearance.Button,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };

            // Add hover effects
            trueRb.MouseEnter += (s, e) => trueRb.BackColor = Color.FromArgb(200, 230, 201);
            trueRb.MouseLeave += (s, e) => trueRb.BackColor = Color.FromArgb(232, 245, 233);

            falseRb.MouseEnter += (s, e) => falseRb.BackColor = Color.FromArgb(255, 205, 210);
            falseRb.MouseLeave += (s, e) => falseRb.BackColor = Color.FromArgb(255, 235, 238);

            // Ensure mutual exclusion
            trueRb.CheckedChanged += (s, e) => { if (trueRb.Checked) falseRb.Checked = false; };
            falseRb.CheckedChanged += (s, e) => { if (falseRb.Checked) trueRb.Checked = false; };

            _answerPanel.Controls.AddRange(new Control[] { instructionLabel, trueRb, falseRb });
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            string userAnswer = GetUserAnswer();
            if (string.IsNullOrEmpty(userAnswer))
            {
                MessageBox.Show("Please select or enter an answer.");
                return;
            }

            _quizGame.SubmitAnswer(userAnswer);
            DisplayCurrentQuestion();
        }

        private string GetUserAnswer()
        {
            var question = _quizGame.GetCurrentQuestion();

            if (question is MultipleChoiceQuestion)
            {
                foreach (RadioButton rb in _answerPanel.Controls.OfType<RadioButton>())
                {
                    if (rb.Checked)
                        return rb.Tag?.ToString() ?? "";
                }
            }
            else if (question is OpenEndedQuestion)
            {
                var textBox = _answerPanel.Controls.Find("answerTextBox", false).FirstOrDefault() as TextBox;
                return textBox?.Text?.Trim() ?? "";
            }
            else if (question is TrueFalseQuestion)
            {
                foreach (RadioButton rb in _answerPanel.Controls.OfType<RadioButton>())
                {
                    if (rb.Checked)
                    {
                        string tag = rb.Tag?.ToString() ?? "";
                        return tag.ToLower() == "true" ? "True" : "False";
                    }
                }
            }

            return "";
        }

        private void ShowResults()
        {
            var result = _quizGame.GetGameResult();
            string message = $"Quiz Complete!\n\n" +
                           $"Score: {result.CorrectAnswers}/{result.TotalQuestions} ({result.ScorePercentage:F1}%)\n" +
                           $"Time: {result.TimeTakenMinutes:F1} minutes\n\n" +
                           $"Would you like to see the correct answers?";

            var dialogResult = MessageBox.Show(message, "Quiz Results", MessageBoxButtons.YesNo);
            
            if (dialogResult == DialogResult.Yes)
            {
                ShowCorrectAnswers(result);
            }

            this.Close();
        }

        private void ShowCorrectAnswers(GameResult result)
        {
            string answers = "Correct Answers:\n\n";
            for (int i = 0; i < result.Questions.Count; i++)
            {
                answers += $"{i + 1}. {result.Questions[i].QuestionText}\n";
                answers += $"   Correct: {result.Questions[i].GetCorrectAnswer()}\n";

                // Format user answer properly based on question type
                string userAnswerDisplay = FormatUserAnswer(result.Questions[i], result.UserResponses[i]);
                answers += $"   Your answer: {userAnswerDisplay} {(result.UserAnswers[i] ? "✓" : "✗")}\n\n";
            }

            MessageBox.Show(answers, "Correct Answers", MessageBoxButtons.OK);
        }

        private string FormatUserAnswer(Question question, string userResponse)
        {
            if (question is MultipleChoiceQuestion mcq)
            {
                if (int.TryParse(userResponse, out int index) && index >= 0 && index < mcq.Options.Count)
                {
                    string[] letters = { "A", "B", "C", "D" };
                    return $"{letters[index]}. {mcq.Options[index]}";
                }
                return userResponse;
            }
            else if (question is TrueFalseQuestion)
            {
                return userResponse; // Already formatted as "True" or "False"
            }
            else if (question is OpenEndedQuestion)
            {
                return userResponse; // User's text input
            }

            return userResponse;
        }
    }
}
