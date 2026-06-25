using CybersecurityAwarenessBot.Controls;
using CybersecurityAwarenessBot.Models;
using CybersecurityAwarenessBot.Services;
using System.Drawing;

namespace CybersecurityAwarenessBot.Forms;

public class ChatbotForm : Form
{
    private const string LogoFileName = "flowrx-logo.png";
    private const string CreatorPhotoFileName = "creator-photo.jpeg";

    private static readonly Color PageBackground = Color.FromArgb(246, 245, 241);
    private static readonly Color Surface = Color.FromArgb(255, 255, 252);
    private static readonly Color Accent = Color.FromArgb(34, 67, 64);
    private static readonly Color SoftAccent = Color.FromArgb(226, 236, 233);
    private static readonly Color WarmText = Color.FromArgb(51, 48, 44);
    private static readonly Color MutedText = Color.FromArgb(102, 96, 88);

    private readonly AudioPlayer _audioPlayer = new();
    private readonly ChatMemory _memory = new();
    private readonly ResponseService _responseService = new();
    private readonly DatabaseService _databaseService = new();
    private readonly ActivityLogService _activityLogService;
    private readonly TaskService _taskService;
    private readonly QuizService _quizService = new();
    private readonly NlpService _nlpService = new();

    private readonly FlowLayoutPanel _chatPanel = new();
    private readonly TextBox _messageTextBox = new();
    private readonly TextBox _nameTextBox = new();
    private readonly Button _sendButton = new();
    private readonly Button _startButton = new();
    private readonly Button _replayVoiceButton = new();
    private readonly Button _reconnectSqlButton = new();
    private readonly TabControl _featureTabs = new();

    private readonly TextBox _taskTitleTextBox = new();
    private readonly TextBox _taskDescriptionTextBox = new();
    private readonly CheckBox _taskReminderCheckBox = new();
    private readonly DateTimePicker _taskReminderPicker = new();
    private readonly ListBox _taskListBox = new();

    private readonly Label _quizQuestionLabel = new();
    private readonly FlowLayoutPanel _quizOptionsPanel = new();
    private readonly Label _quizFeedbackLabel = new();
    private readonly Label _quizScoreLabel = new();
    private readonly Button _submitQuizButton = new();

    private readonly Label _databaseStatusLabel = new();
    private readonly ListBox _activityListBox = new();


    public ChatbotForm()
    {
        _activityLogService = new ActivityLogService(_databaseService);
        _taskService = new TaskService(_databaseService, _activityLogService);

        Text = "FLO WRX - Cybersecurity Awareness Assistant";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1120, 760);
        BackColor = PageBackground;
        Font = new Font("Segoe UI", 10);

        BuildLayout();
        Shown += ChatbotForm_Shown;
    }


    private void BuildLayout()
    {
        TableLayoutPanel mainLayout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(22),
            BackColor = PageBackground
        };

        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 164));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 82));

        mainLayout.Controls.Add(BuildHeaderPanel(), 0, 0);
        mainLayout.Controls.Add(BuildNamePanel(), 0, 1);
        mainLayout.Controls.Add(BuildWorkArea(), 0, 2);
        mainLayout.Controls.Add(BuildInputPanel(), 0, 3);

        Controls.Add(mainLayout);
    }


    private Control BuildWorkArea()
    {
        TableLayoutPanel workArea = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            BackColor = PageBackground
        };

        workArea.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
        workArea.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));

        workArea.Controls.Add(BuildChatArea(), 0, 0);
        workArea.Controls.Add(BuildFeatureTabs(), 1, 0);

        return workArea;
    }


    private Control BuildHeaderPanel()
    {
        RoundedPanel headerPanel = new()
        {
            Dock = DockStyle.Fill,
            CornerRadius = 24,
            FillColor = Color.FromArgb(62, 58, 52),
            Padding = new Padding(18),
            Margin = new Padding(0, 0, 0, 14)
        };

        TableLayoutPanel headerLayout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            BackColor = Color.Transparent
        };

        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 116));
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 118));
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        PictureBox logoBox = new()
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            Margin = new Padding(0, 0, 16, 0)
        };

        LoadImage(logoBox, LogoFileName);

        RoundedPanel creatorPanel = new()
        {
            Dock = DockStyle.Fill,
            CornerRadius = 18,
            FillColor = Color.FromArgb(83, 78, 70),
            Padding = new Padding(7),
            Margin = new Padding(0, 0, 16, 0)
        };

        PictureBox creatorBox = new()
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.FromArgb(83, 78, 70)
        };

        LoadImage(creatorBox, CreatorPhotoFileName);
        creatorPanel.Controls.Add(creatorBox);

        Label titleLabel = new()
        {
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 23, FontStyle.Bold),
            Text = "FLO WRX\r\nCybersecurity Awareness Assistant",
            TextAlign = ContentAlignment.MiddleLeft
        };

        headerLayout.Controls.Add(logoBox, 0, 0);
        headerLayout.Controls.Add(creatorPanel, 1, 0);
        headerLayout.Controls.Add(titleLabel, 2, 0);
        headerPanel.Controls.Add(headerLayout);

        return headerPanel;
    }


    private Control BuildNamePanel()
    {
        RoundedPanel namePanel = new()
        {
            Dock = DockStyle.Fill,
            CornerRadius = 20,
            FillColor = Surface,
            Padding = new Padding(16, 12, 16, 12),
            Margin = new Padding(0, 0, 0, 14)
        };

        TableLayoutPanel nameLayout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 5,
            BackColor = Color.Transparent
        };

        nameLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95));
        nameLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));
        nameLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        nameLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        nameLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        Label nameLabel = new()
        {
            Text = "Your name",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = WarmText,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _nameTextBox.Dock = DockStyle.Fill;
        _nameTextBox.Font = new Font("Segoe UI", 10);
        _nameTextBox.PlaceholderText = "Enter your name";
        _nameTextBox.BorderStyle = BorderStyle.FixedSingle;
        _nameTextBox.Margin = new Padding(0, 3, 12, 3);

        StyleButton(_startButton, "Start Chat", Accent);
        _startButton.Click += StartButton_Click;

        StyleButton(_replayVoiceButton, "Replay Voice", Color.FromArgb(92, 86, 78));
        _replayVoiceButton.Click += ReplayVoiceButton_Click;

        nameLayout.Controls.Add(nameLabel, 0, 0);
        nameLayout.Controls.Add(_nameTextBox, 1, 0);
        nameLayout.Controls.Add(_startButton, 2, 0);
        nameLayout.Controls.Add(_replayVoiceButton, 3, 0);
        namePanel.Controls.Add(nameLayout);

        return namePanel;
    }


    private Control BuildChatArea()
    {
        RoundedPanel chatCard = new()
        {
            Dock = DockStyle.Fill,
            CornerRadius = 24,
            FillColor = Surface,
            Padding = new Padding(16),
            Margin = new Padding(0, 0, 14, 14)
        };

        _chatPanel.Dock = DockStyle.Fill;
        _chatPanel.AutoScroll = true;
        _chatPanel.FlowDirection = FlowDirection.TopDown;
        _chatPanel.WrapContents = false;
        _chatPanel.BackColor = Surface;
        _chatPanel.Padding = new Padding(4);

        chatCard.Controls.Add(_chatPanel);
        return chatCard;
    }


    private Control BuildFeatureTabs()
    {
        _featureTabs.Dock = DockStyle.Fill;
        _featureTabs.Font = new Font("Segoe UI", 9);
        _featureTabs.Controls.Add(BuildTaskTab());
        _featureTabs.Controls.Add(BuildQuizTab());
        _featureTabs.Controls.Add(BuildActivityTab());

        return _featureTabs;
    }


    private TabPage BuildTaskTab()
    {
        TabPage taskTab = new("Tasks")
        {
            Name = "Tasks"
        };

        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            RowCount = 5,
            Padding = new Padding(10),
            BackColor = Surface
        };

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        _taskTitleTextBox.Dock = DockStyle.Fill;
        _taskTitleTextBox.PlaceholderText = "Task title, e.g. Enable 2FA";

        _taskDescriptionTextBox.Dock = DockStyle.Fill;
        _taskDescriptionTextBox.Multiline = true;
        _taskDescriptionTextBox.PlaceholderText = "Task description";

        FlowLayoutPanel reminderPanel = new()
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight
        };

        _taskReminderCheckBox.Text = "Reminder";
        _taskReminderCheckBox.AutoSize = true;
        _taskReminderCheckBox.CheckedChanged += TaskReminderCheckBox_CheckedChanged;

        _taskReminderPicker.Format = DateTimePickerFormat.Custom;
        _taskReminderPicker.CustomFormat = "dd MMM yyyy HH:mm";
        _taskReminderPicker.Width = 165;
        _taskReminderPicker.Enabled = false;

        reminderPanel.Controls.Add(_taskReminderCheckBox);
        reminderPanel.Controls.Add(_taskReminderPicker);

        FlowLayoutPanel buttonPanel = new()
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight
        };

        Button addTaskButton = CreateSmallButton("Add");
        addTaskButton.Click += AddTaskButton_Click;

        Button refreshTaskButton = CreateSmallButton("View");
        refreshTaskButton.Click += RefreshTaskButton_Click;

        Button completeTaskButton = CreateSmallButton("Complete");
        completeTaskButton.Click += CompleteTaskButton_Click;

        Button deleteTaskButton = CreateSmallButton("Delete");
        deleteTaskButton.Click += DeleteTaskButton_Click;

        buttonPanel.Controls.Add(addTaskButton);
        buttonPanel.Controls.Add(refreshTaskButton);
        buttonPanel.Controls.Add(completeTaskButton);
        buttonPanel.Controls.Add(deleteTaskButton);

        _taskListBox.Dock = DockStyle.Fill;
        _taskListBox.Font = new Font("Segoe UI", 9);

        layout.Controls.Add(_taskTitleTextBox, 0, 0);
        layout.Controls.Add(_taskDescriptionTextBox, 0, 1);
        layout.Controls.Add(reminderPanel, 0, 2);
        layout.Controls.Add(buttonPanel, 0, 3);
        layout.Controls.Add(_taskListBox, 0, 4);
        taskTab.Controls.Add(layout);

        return taskTab;
    }


    private TabPage BuildQuizTab()
    {
        TabPage quizTab = new("Quiz")
        {
            Name = "Quiz"
        };

        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            RowCount = 5,
            Padding = new Padding(10),
            BackColor = Surface
        };

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 90));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 88));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

        FlowLayoutPanel topButtons = new()
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight
        };

        Button startQuizButton = CreateSmallButton("Start Quiz");
        startQuizButton.Click += StartQuizButton_Click;

        StyleButton(_submitQuizButton, "Submit", Accent);
        _submitQuizButton.Width = 95;
        _submitQuizButton.Enabled = false;
        _submitQuizButton.Click += SubmitQuizButton_Click;

        topButtons.Controls.Add(startQuizButton);
        topButtons.Controls.Add(_submitQuizButton);

        _quizQuestionLabel.Dock = DockStyle.Fill;
        _quizQuestionLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        _quizQuestionLabel.ForeColor = WarmText;
        _quizQuestionLabel.Text = "Start the quiz to test your cybersecurity awareness.";

        _quizOptionsPanel.Dock = DockStyle.Fill;
        _quizOptionsPanel.FlowDirection = FlowDirection.TopDown;
        _quizOptionsPanel.WrapContents = false;
        _quizOptionsPanel.AutoScroll = true;

        _quizFeedbackLabel.Dock = DockStyle.Fill;
        _quizFeedbackLabel.ForeColor = MutedText;
        _quizFeedbackLabel.Text = "Feedback will appear here after each answer.";

        _quizScoreLabel.Dock = DockStyle.Fill;
        _quizScoreLabel.ForeColor = WarmText;
        _quizScoreLabel.Text = "Score: 0/0";

        layout.Controls.Add(topButtons, 0, 0);
        layout.Controls.Add(_quizQuestionLabel, 0, 1);
        layout.Controls.Add(_quizOptionsPanel, 0, 2);
        layout.Controls.Add(_quizFeedbackLabel, 0, 3);
        layout.Controls.Add(_quizScoreLabel, 0, 4);
        quizTab.Controls.Add(layout);

        return quizTab;
    }


    private TabPage BuildActivityTab()
    {
        TabPage activityTab = new("Activity")
        {
            Name = "Activity"
        };

        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            Padding = new Padding(10),
            BackColor = Surface
        };

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        _databaseStatusLabel.Dock = DockStyle.Fill;
        _databaseStatusLabel.ForeColor = MutedText;
        _databaseStatusLabel.Text = "Checking MySQL...";

        FlowLayoutPanel activityButtons = new()
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight
        };

        Button refreshActivityButton = CreateSmallButton("Refresh Log");
        refreshActivityButton.Click += RefreshActivityButton_Click;

        StyleButton(_reconnectSqlButton, "Reconnect SQL", Color.FromArgb(92, 86, 78));
        _reconnectSqlButton.Width = 120;
        _reconnectSqlButton.Dock = DockStyle.None;
        _reconnectSqlButton.Click += ReconnectSqlButton_Click;

        activityButtons.Controls.Add(refreshActivityButton);
        activityButtons.Controls.Add(_reconnectSqlButton);

        _activityListBox.Dock = DockStyle.Fill;
        _activityListBox.Font = new Font("Segoe UI", 9);

        layout.Controls.Add(_databaseStatusLabel, 0, 0);
        layout.Controls.Add(activityButtons, 0, 1);
        layout.Controls.Add(_activityListBox, 0, 2);
        activityTab.Controls.Add(layout);

        return activityTab;
    }


    private Control BuildInputPanel()
    {
        RoundedPanel inputPanel = new()
        {
            Dock = DockStyle.Fill,
            CornerRadius = 22,
            FillColor = Surface,
            Padding = new Padding(16),
            Margin = new Padding(0)
        };

        TableLayoutPanel inputLayout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            BackColor = Color.Transparent
        };

        inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112));

        _messageTextBox.Dock = DockStyle.Fill;
        _messageTextBox.Font = new Font("Segoe UI", 11);
        _messageTextBox.PlaceholderText = "Type your message here...";
        _messageTextBox.BorderStyle = BorderStyle.None;
        _messageTextBox.Enabled = false;
        _messageTextBox.Margin = new Padding(0, 13, 14, 0);
        _messageTextBox.KeyDown += MessageTextBox_KeyDown;

        StyleButton(_sendButton, "Send", Accent);
        _sendButton.Enabled = false;
        _sendButton.Click += SendButton_Click;

        inputLayout.Controls.Add(_messageTextBox, 0, 0);
        inputLayout.Controls.Add(_sendButton, 1, 0);
        inputPanel.Controls.Add(inputLayout);

        return inputPanel;
    }


    private async void ChatbotForm_Shown(object? sender, EventArgs e)
    {
        _audioPlayer.TryPlayGreeting();
        AddBotMessage("Hello! Welcome to FLO WRX.");
        AddBotMessage("You can chat, add cybersecurity tasks, start the quiz, or ask to see the activity log.");

        await _databaseService.InitializeAsync();
        _databaseStatusLabel.Text = _databaseService.StatusMessage;

        await _activityLogService.AddAsync("Application opened");
        await RefreshTasksAsync();
        await RefreshActivityLogAsync();
    }


    private void StartButton_Click(object? sender, EventArgs e)
    {
        string name = _nameTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            AddBotMessage("Please enter your name first so I can personalise the chat.");
            return;
        }

        _memory.UserName = name;
        _nameTextBox.Enabled = false;
        _startButton.Enabled = false;
        _messageTextBox.Enabled = true;
        _sendButton.Enabled = true;
        _messageTextBox.Focus();

        AddUserMessage($"My name is {name}.");
        AddBotMessage($"Welcome, {name}. You can ask for tips, add tasks, start a quiz, or say 'show activity log'.");
    }


    private async void SendButton_Click(object? sender, EventArgs e)
    {
        await SendMessageAsync();
    }


    private async void MessageTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            await SendMessageAsync();
        }
    }


    private void ReplayVoiceButton_Click(object? sender, EventArgs e)
    {
        if (!_audioPlayer.TryPlayGreeting(out string message))
        {
            AddBotMessage(message);
        }
    }


    private async Task SendMessageAsync()
    {
        string message = _messageTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(message))
        {
            AddBotMessage("Please type a question before pressing Send.");
            return;
        }

        AddUserMessage(message);
        _messageTextBox.Clear();

        if (message.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
            message.Equals("quit", StringComparison.OrdinalIgnoreCase))
        {
            AddBotMessage($"Goodbye, {_memory.UserName}. Stay safe online.");
            _messageTextBox.Enabled = false;
            _sendButton.Enabled = false;
            return;
        }

        NlpCommand command = _nlpService.ReadCommand(message);
        if (command.Type != NlpCommandType.None)
        {
            await HandleNlpCommandAsync(command);
            return;
        }

        string response = _responseService.GetResponse(message, _memory);
        AddBotMessage(response);
    }


    private async Task HandleNlpCommandAsync(NlpCommand command)
    {
        switch (command.Type)
        {
            case NlpCommandType.AddTask:
                await AddTaskFromCommandAsync(command);
                break;
            case NlpCommandType.ViewTasks:
                await RefreshTasksAsync();
                _featureTabs.SelectedTab = _featureTabs.TabPages["Tasks"];
                AddBotMessage("I opened your cybersecurity task list.");
                await _activityLogService.AddAsync("NLP command recognized: view tasks");
                break;
            case NlpCommandType.CompleteTask:
                await CompleteTaskFromCommandAsync(command.TaskId);
                break;
            case NlpCommandType.DeleteTask:
                await DeleteTaskFromCommandAsync(command.TaskId);
                break;
            case NlpCommandType.ShowActivityLog:
                await RefreshActivityLogAsync();
                _featureTabs.SelectedTab = _featureTabs.TabPages["Activity"];
                AddBotMessage("Here are the most recent actions I found for you.");
                await _activityLogService.AddAsync("NLP command recognized: show activity log");
                break;
            case NlpCommandType.StartQuiz:
                await StartQuizAsync();
                await _activityLogService.AddAsync("NLP command recognized: start quiz");
                break;
        }
    }


    private async Task AddTaskFromCommandAsync(NlpCommand command)
    {
        CyberTask task = new()
        {
            Title = command.TaskTitle,
            Description = command.TaskDescription,
            HasReminder = command.HasReminder,
            ReminderText = command.ReminderText,
            ReminderDate = command.ReminderDate,
            CreatedAt = DateTime.Now
        };

        (bool _, string message) = await _taskService.AddTaskAsync(task);

        if (command.HasReminder)
        {
            await _activityLogService.AddAsync($"Reminder created for task: {command.TaskTitle}");
        }

        await RefreshTasksAsync();
        await RefreshActivityLogAsync();
        _featureTabs.SelectedTab = _featureTabs.TabPages["Tasks"];
        AddBotMessage(message);
    }


    private async void AddTaskButton_Click(object? sender, EventArgs e)
    {
        string title = _taskTitleTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            AddBotMessage("Please enter a task title first.");
            return;
        }

        CyberTask task = new()
        {
            Title = title,
            Description = string.IsNullOrWhiteSpace(_taskDescriptionTextBox.Text)
                ? "No description added."
                : _taskDescriptionTextBox.Text.Trim(),
            HasReminder = _taskReminderCheckBox.Checked,
            ReminderText = _taskReminderCheckBox.Checked ? "Manual reminder" : null,
            ReminderDate = _taskReminderCheckBox.Checked ? _taskReminderPicker.Value : null,
            CreatedAt = DateTime.Now
        };

        (bool _, string message) = await _taskService.AddTaskAsync(task);

        if (task.HasReminder)
        {
            await _activityLogService.AddAsync($"Reminder created for task: {task.Title}");
        }

        _taskTitleTextBox.Clear();
        _taskDescriptionTextBox.Clear();
        _taskReminderCheckBox.Checked = false;

        await RefreshTasksAsync();
        await RefreshActivityLogAsync();
        AddBotMessage(message);
    }


    private async void RefreshTaskButton_Click(object? sender, EventArgs e)
    {
        await RefreshTasksAsync();
        AddBotMessage("Task list refreshed.");
    }


    private async void CompleteTaskButton_Click(object? sender, EventArgs e)
    {
        int? taskId = GetSelectedTaskId();
        await CompleteTaskFromCommandAsync(taskId);
    }


    private async Task CompleteTaskFromCommandAsync(int? taskId)
    {
        if (taskId is null)
        {
            AddBotMessage("Please select a task or type a task number, like 'complete task 2'.");
            return;
        }

        (bool _, string message) = await _taskService.CompleteTaskAsync(taskId.Value);
        await RefreshTasksAsync();
        await RefreshActivityLogAsync();
        AddBotMessage(message);
    }


    private async void DeleteTaskButton_Click(object? sender, EventArgs e)
    {
        int? taskId = GetSelectedTaskId();
        await DeleteTaskFromCommandAsync(taskId);
    }


    private async Task DeleteTaskFromCommandAsync(int? taskId)
    {
        if (taskId is null)
        {
            AddBotMessage("Please select a task or type a task number, like 'delete task 2'.");
            return;
        }

        (bool _, string message) = await _taskService.DeleteTaskAsync(taskId.Value);
        await RefreshTasksAsync();
        await RefreshActivityLogAsync();
        AddBotMessage(message);
    }


    private async Task RefreshTasksAsync()
    {
        List<CyberTask> tasks = await _taskService.GetTasksAsync();
        _taskListBox.Items.Clear();

        foreach (CyberTask task in tasks)
        {
            _taskListBox.Items.Add(task);
        }
    }


    private int? GetSelectedTaskId()
    {
        return _taskListBox.SelectedItem is CyberTask selectedTask ? selectedTask.Id : null;
    }


    private void TaskReminderCheckBox_CheckedChanged(object? sender, EventArgs e)
    {
        _taskReminderPicker.Enabled = _taskReminderCheckBox.Checked;
    }


    private async void StartQuizButton_Click(object? sender, EventArgs e)
    {
        await StartQuizAsync();
    }


    private async Task StartQuizAsync()
    {
        _featureTabs.SelectedTab = _featureTabs.TabPages["Quiz"];
        _quizService.Start();
        DisplayCurrentQuizQuestion();
        _submitQuizButton.Enabled = true;
        _quizFeedbackLabel.Text = "Choose an answer and press Submit.";
        _quizScoreLabel.Text = $"Score: 0/{_quizService.TotalQuestions}";

        AddBotMessage("Cybersecurity quiz started. Answer one question at a time.");
        await _activityLogService.AddAsync("Quiz started");
        await RefreshActivityLogAsync();
    }


    private async void SubmitQuizButton_Click(object? sender, EventArgs e)
    {
        RadioButton? selectedOption = _quizOptionsPanel.Controls
            .OfType<RadioButton>()
            .FirstOrDefault(option => option.Checked);

        if (selectedOption is null)
        {
            _quizFeedbackLabel.Text = "Please choose an answer first.";
            return;
        }

        int selectedIndex = (int)selectedOption.Tag!;
        QuizAnswerResult result = _quizService.SubmitAnswer(selectedIndex);

        _quizFeedbackLabel.Text = result.Feedback;
        _quizScoreLabel.Text = $"Score: {result.Score}/{result.TotalQuestions}";
        AddBotMessage(result.Feedback);

        if (result.IsFinished)
        {
            _quizQuestionLabel.Text = "Quiz complete.";
            _quizOptionsPanel.Controls.Clear();
            _submitQuizButton.Enabled = false;
            await _activityLogService.AddAsync($"Quiz completed with score {result.Score}/{result.TotalQuestions}");
        }
        else
        {
            DisplayCurrentQuizQuestion();
        }

        await RefreshActivityLogAsync();
    }


    private void DisplayCurrentQuizQuestion()
    {
        QuizQuestion? question = _quizService.CurrentQuestion;

        if (question is null)
        {
            return;
        }

        _quizQuestionLabel.Text = question.QuestionText;
        _quizOptionsPanel.Controls.Clear();

        for (int index = 0; index < question.Options.Count; index++)
        {
            RadioButton optionButton = new()
            {
                Text = question.Options[index],
                Tag = index,
                AutoSize = true,
                MaximumSize = new Size(330, 0),
                Margin = new Padding(4, 6, 4, 6)
            };

            _quizOptionsPanel.Controls.Add(optionButton);
        }
    }


    private async void RefreshActivityButton_Click(object? sender, EventArgs e)
    {
        await RefreshActivityLogAsync();
        AddBotMessage("Activity log refreshed.");
    }


    private async void ReconnectSqlButton_Click(object? sender, EventArgs e)
    {
        await _databaseService.InitializeAsync();
        _databaseStatusLabel.Text = _databaseService.StatusMessage;

        if (_databaseService.IsReady)
        {
            AddBotMessage("Local MySQL is connected now. New tasks will be saved in the database.");
        }
        else
        {
            AddBotMessage("I still cannot connect to MySQL. Please make sure the local MySQL server is running.");
        }

        await RefreshTasksAsync();
        await RefreshActivityLogAsync();
    }


    private async Task RefreshActivityLogAsync()
    {
        List<ActivityLogEntry> entries = await _activityLogService.GetRecentAsync(10);
        _activityListBox.Items.Clear();

        foreach (ActivityLogEntry entry in entries)
        {
            _activityListBox.Items.Add(entry);
        }

        _databaseStatusLabel.Text = _databaseService.StatusMessage;
    }


    private static Button CreateSmallButton(string text)
    {
        Button button = new();
        StyleButton(button, text, Accent);
        button.Width = 95;
        button.Height = 32;
        button.Dock = DockStyle.None;
        return button;
    }


    private static void StyleButton(Button button, string text, Color backColor)
    {
        button.Text = text;
        button.Dock = DockStyle.Fill;
        button.Height = 38;
        button.Margin = new Padding(5, 3, 5, 3);
        button.BackColor = backColor;
        button.ForeColor = Color.White;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        button.Cursor = Cursors.Hand;
    }


    private static void LoadImage(PictureBox pictureBox, string fileName)
    {
        string imagePath = Path.Combine(AppContext.BaseDirectory, "Assets", fileName);

        if (File.Exists(imagePath))
        {
            pictureBox.Image = Image.FromFile(imagePath);
        }
    }


    private void AddBotMessage(string message)
    {
        AppendMessage("FLO WRX", message, false);
    }


    private void AddUserMessage(string message)
    {
        AppendMessage(_memory.UserName, message, true);
    }


    private void AppendMessage(string sender, string message, bool isUser)
    {
        int rowWidth = Math.Max(400, _chatPanel.ClientSize.Width - 32);
        int maximumTextWidth = Math.Max(260, rowWidth - 190);
        string text = $"{sender}: {message}";

        Font bubbleFont = new("Segoe UI", 10);
        Size textSize = TextRenderer.MeasureText(
            text,
            bubbleFont,
            new Size(maximumTextWidth, int.MaxValue),
            TextFormatFlags.WordBreak);

        RoundedPanel bubble = new()
        {
            CornerRadius = 18,
            FillColor = isUser ? Accent : SoftAccent,
            Size = new Size(textSize.Width + 28, textSize.Height + 24),
            Padding = new Padding(14, 12, 14, 12)
        };

        Label messageLabel = new()
        {
            Text = text,
            Font = bubbleFont,
            ForeColor = isUser ? Color.White : WarmText,
            BackColor = Color.Transparent,
            Location = new Point(14, 12),
            Size = textSize
        };

        Panel row = new()
        {
            Width = rowWidth,
            Height = bubble.Height + 10,
            Margin = new Padding(0, 0, 0, 10),
            BackColor = Surface
        };

        bubble.Location = isUser
            ? new Point(row.Width - bubble.Width - 8, 0)
            : new Point(8, 0);

        bubble.Controls.Add(messageLabel);
        row.Controls.Add(bubble);
        _chatPanel.Controls.Add(row);
        _chatPanel.ScrollControlIntoView(row);
    }
}
