using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services;

public class ResponseService
{
    private delegate string ReplyMethod(ChatMemory memory, SentimentType sentiment);

    private readonly Random _random = new();
    private readonly Dictionary<string, ReplyMethod> _keywordReplies;
    private readonly Dictionary<string, List<string>> _topicResponses;
    private readonly List<string> _keywords;


    public ResponseService()
    {
        _topicResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["password"] = new List<string>
            {
                "Use strong, unique passwords for every account. Avoid using names, birthdays, or simple words.",
                "A password manager can help you store strong passwords instead of trying to remember all of them.",
                "A long passphrase can be easier to remember and harder for someone else to guess.",
                "Do not reuse the same password on many accounts, because one leak can affect everything."
            },
            ["phishing"] = new List<string>
            {
                "Be careful with emails or messages asking for urgent action. Scammers often try to rush you.",
                "Check the sender address before clicking links or downloading attachments.",
                "If a message looks suspicious, open the official website yourself instead of using the link.",
                "Phishing messages often use fear, prizes, or fake account warnings to get your attention."
            },
            ["scam"] = new List<string>
            {
                "If an offer sounds too good to be true, it is safer to double-check it first.",
                "Never share OTPs, banking PINs, or passwords with anyone who contacts you unexpectedly.",
                "Scammers often use pressure. Take your time before sending money or personal information.",
                "Before trusting a message, check if the company name, phone number, and website are real."
            },
            ["privacy"] = new List<string>
            {
                "Review privacy settings on your social media and online accounts.",
                "Limit how much personal information you post online, especially your location and contact details.",
                "Use multi-factor authentication to add extra protection to private accounts.",
                "Be careful with apps that ask for permissions they do not really need."
            },
            ["malware"] = new List<string>
            {
                "Only download files and apps from trusted websites or official app stores.",
                "Keep your antivirus software and operating system updated.",
                "Avoid opening unexpected attachments because they may contain harmful software.",
                "Malware can hide inside fake downloads, cracked software, and unsafe email attachments."
            },
            ["safe browsing"] = new List<string>
            {
                "Use websites that show HTTPS, especially when logging in or making payments.",
                "Avoid unknown pop-ups, fake download buttons, and suspicious links.",
                "Keep your browser updated so it has the latest security fixes.",
                "Do not enter personal details on websites that look copied, rushed, or badly spelled."
            },
            ["public wifi"] = new List<string>
            {
                "Avoid logging into banking or school accounts on public Wi-Fi unless you trust the network.",
                "Public Wi-Fi can be risky because other people may try to watch traffic on the same network.",
                "Use a VPN on public Wi-Fi if you need extra privacy while browsing."
            },
            ["vpn"] = new List<string>
            {
                "A VPN can help protect your browsing on public networks.",
                "A VPN does not make you completely safe, but it can add privacy on unsafe networks.",
                "Use a trusted VPN provider and still avoid suspicious websites."
            },
            ["antivirus"] = new List<string>
            {
                "Antivirus software helps detect and remove harmful programs.",
                "Keep your antivirus updated so it can recognise newer threats.",
                "Antivirus is useful, but you still need safe browsing habits."
            },
            ["firewall"] = new List<string>
            {
                "A firewall helps block unwanted network traffic from reaching your device.",
                "Firewalls are useful because they add a layer of protection between your device and the internet.",
                "Do not turn off your firewall unless you understand why it is needed."
            },
            ["two factor authentication"] = new List<string>
            {
                "Two-factor authentication adds a second step after your password.",
                "Use an authenticator app where possible, because it is usually safer than SMS codes.",
                "Never share your one-time codes with anyone, even if they claim to be support staff."
            },
            ["backup"] = new List<string>
            {
                "Backups protect your files if your device is lost, damaged, or infected.",
                "Keep important files backed up in more than one place if possible.",
                "A backup is very helpful if ransomware locks your files."
            },
            ["software update"] = new List<string>
            {
                "Software updates often fix security weaknesses.",
                "Update your phone, computer, browser, and apps when updates are available.",
                "Delaying updates can leave your device open to known attacks."
            },
            ["social media"] = new List<string>
            {
                "Be careful with friend requests from people you do not know.",
                "Do not post too much personal information, like your address or daily routine.",
                "Check your privacy settings so only the right people can see your posts."
            },
            ["online banking"] = new List<string>
            {
                "For online banking, use a strong password and multi-factor authentication.",
                "Never share your banking PIN, password, or OTP with anyone.",
                "Avoid online banking on public Wi-Fi unless you have a secure connection."
            },
            ["online shopping"] = new List<string>
            {
                "Shop from trusted websites and check for HTTPS before paying.",
                "Be careful with deals that look too cheap or too urgent.",
                "Use secure payment methods and avoid saving card details on unknown sites."
            },
            ["fake website"] = new List<string>
            {
                "Fake websites may copy real brands to steal your details.",
                "Check the website address carefully before entering login or payment information.",
                "Look for spelling mistakes, strange links, and unusual payment requests."
            },
            ["identity theft"] = new List<string>
            {
                "Identity theft happens when someone uses your personal information without permission.",
                "Protect your ID numbers, banking details, passwords, and personal documents.",
                "If you suspect identity theft, change passwords and report it as soon as possible."
            },
            ["email safety"] = new List<string>
            {
                "Do not open unexpected attachments from unknown senders.",
                "Check the sender address and the message wording before trusting an email.",
                "Be careful when an email asks for passwords, payments, or urgent action."
            },
            ["suspicious link"] = new List<string>
            {
                "Do not click suspicious links, especially in unexpected messages.",
                "Hover over links to check where they lead before opening them.",
                "If you are unsure, search for the official website instead of using the link."
            },
            ["usb"] = new List<string>
            {
                "Do not plug unknown USB drives into your device.",
                "USB devices can carry malware, even if they look normal.",
                "Only use USB drives from people or places you trust."
            },
            ["ransomware"] = new List<string>
            {
                "Ransomware locks files and demands payment to unlock them.",
                "Backups are one of the best ways to recover from ransomware.",
                "Avoid suspicious downloads and attachments to reduce ransomware risk."
            },
            ["cookies"] = new List<string>
            {
                "Cookies can remember website settings, but some are used for tracking.",
                "Review cookie settings when websites give you the option.",
                "Clearing cookies can help reduce some tracking, but it may sign you out of websites."
            },
            ["cyberbullying"] = new List<string>
            {
                "Cyberbullying should be reported and not ignored.",
                "Keep evidence like screenshots if someone is harassing you online.",
                "Block abusive users and speak to a trusted person if the situation continues."
            },
            ["device lock"] = new List<string>
            {
                "Use a lock screen password, PIN, fingerprint, or face unlock on your device.",
                "A locked device helps protect your information if it is lost or stolen.",
                "Do not share your device PIN with people you do not fully trust."
            },
            ["cloud storage"] = new List<string>
            {
                "Cloud storage can be safe if you use a strong password and multi-factor authentication.",
                "Do not upload private documents to accounts that are not properly protected.",
                "Check sharing settings so files are not public by mistake."
            }
        };

        _keywordReplies = new Dictionary<string, ReplyMethod>(StringComparer.OrdinalIgnoreCase)
        {
            ["password"] = (memory, sentiment) => GiveTopicReply("password", memory, sentiment),
            ["password manager"] = (memory, sentiment) => GiveTopicReply("password", memory, sentiment),
            ["passphrase"] = (memory, sentiment) => GiveTopicReply("password", memory, sentiment),
            ["phishing"] = (memory, sentiment) => GiveTopicReply("phishing", memory, sentiment),
            ["spear phishing"] = (memory, sentiment) => GiveTopicReply("phishing", memory, sentiment),
            ["smishing"] = (memory, sentiment) => GiveTopicReply("phishing", memory, sentiment),
            ["vishing"] = (memory, sentiment) => GiveTopicReply("phishing", memory, sentiment),
            ["scam"] = (memory, sentiment) => GiveTopicReply("scam", memory, sentiment),
            ["privacy"] = (memory, sentiment) => GiveTopicReply("privacy", memory, sentiment),
            ["malware"] = (memory, sentiment) => GiveTopicReply("malware", memory, sentiment),
            ["virus"] = (memory, sentiment) => GiveTopicReply("malware", memory, sentiment),
            ["safe browsing"] = (memory, sentiment) => GiveTopicReply("safe browsing", memory, sentiment),
            ["browser"] = (memory, sentiment) => GiveTopicReply("safe browsing", memory, sentiment),
            ["https"] = (memory, sentiment) => GiveTopicReply("safe browsing", memory, sentiment),
            ["public wifi"] = (memory, sentiment) => GiveTopicReply("public wifi", memory, sentiment),
            ["public wi-fi"] = (memory, sentiment) => GiveTopicReply("public wifi", memory, sentiment),
            ["vpn"] = (memory, sentiment) => GiveTopicReply("vpn", memory, sentiment),
            ["antivirus"] = (memory, sentiment) => GiveTopicReply("antivirus", memory, sentiment),
            ["firewall"] = (memory, sentiment) => GiveTopicReply("firewall", memory, sentiment),
            ["2fa"] = (memory, sentiment) => GiveTopicReply("two factor authentication", memory, sentiment),
            ["mfa"] = (memory, sentiment) => GiveTopicReply("two factor authentication", memory, sentiment),
            ["two factor"] = (memory, sentiment) => GiveTopicReply("two factor authentication", memory, sentiment),
            ["multi factor"] = (memory, sentiment) => GiveTopicReply("two factor authentication", memory, sentiment),
            ["otp"] = (memory, sentiment) => GiveTopicReply("two factor authentication", memory, sentiment),
            ["backup"] = (memory, sentiment) => GiveTopicReply("backup", memory, sentiment),
            ["update"] = (memory, sentiment) => GiveTopicReply("software update", memory, sentiment),
            ["software update"] = (memory, sentiment) => GiveTopicReply("software update", memory, sentiment),
            ["social media"] = (memory, sentiment) => GiveTopicReply("social media", memory, sentiment),
            ["online banking"] = (memory, sentiment) => GiveTopicReply("online banking", memory, sentiment),
            ["banking"] = (memory, sentiment) => GiveTopicReply("online banking", memory, sentiment),
            ["online shopping"] = (memory, sentiment) => GiveTopicReply("online shopping", memory, sentiment),
            ["shopping"] = (memory, sentiment) => GiveTopicReply("online shopping", memory, sentiment),
            ["fake website"] = (memory, sentiment) => GiveTopicReply("fake website", memory, sentiment),
            ["identity theft"] = (memory, sentiment) => GiveTopicReply("identity theft", memory, sentiment),
            ["email"] = (memory, sentiment) => GiveTopicReply("email safety", memory, sentiment),
            ["attachment"] = (memory, sentiment) => GiveTopicReply("email safety", memory, sentiment),
            ["suspicious link"] = (memory, sentiment) => GiveTopicReply("suspicious link", memory, sentiment),
            ["unknown link"] = (memory, sentiment) => GiveTopicReply("suspicious link", memory, sentiment),
            ["usb"] = (memory, sentiment) => GiveTopicReply("usb", memory, sentiment),
            ["ransomware"] = (memory, sentiment) => GiveTopicReply("ransomware", memory, sentiment),
            ["cookies"] = (memory, sentiment) => GiveTopicReply("cookies", memory, sentiment),
            ["tracking"] = (memory, sentiment) => GiveTopicReply("cookies", memory, sentiment),
            ["cyberbullying"] = (memory, sentiment) => GiveTopicReply("cyberbullying", memory, sentiment),
            ["lock screen"] = (memory, sentiment) => GiveTopicReply("device lock", memory, sentiment),
            ["device lock"] = (memory, sentiment) => GiveTopicReply("device lock", memory, sentiment),
            ["cloud storage"] = (memory, sentiment) => GiveTopicReply("cloud storage", memory, sentiment)
        };

        _keywords = _keywordReplies.Keys
            .OrderByDescending(keyword => keyword.Length)
            .ToList();
    }



    public string GetResponse(string input, ChatMemory memory)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return "Please type a question so I can help you.";
        }

        string normalized = CleanInput(input);
        SentimentType sentiment = DetectSentiment(normalized);

        if (TryRememberFavouriteTopic(normalized, memory, out string rememberedTopic))
        {
            memory.LastTopic = rememberedTopic;
            return $"{GetMoodOpening(sentiment)}Great, {memory.UserName}. I'll remember that you're interested in {rememberedTopic}. {GetRandomResponse(rememberedTopic)}";
        }

        if (IsFollowUpQuestion(normalized))
        {
            return AnswerFollowUp(memory, sentiment);
        }

        if (IsGreeting(normalized))
        {
            return $"Hello {memory.UserName}! How can I help you today?";
        }

        if (normalized.Contains("how are you"))
        {
            return $"I'm doing great, {memory.UserName}. Thanks for asking. How are you doing today?";
        }

        if (IsPositiveReply(normalized))
        {
            return $"That's good to hear, {memory.UserName}. Is there anything you'd like to learn about cybersecurity today?";
        }

        if (IsNegativeReply(normalized))
        {
            return $"I'm sorry you're feeling that way, {memory.UserName}. Maybe I can help with some useful cybersecurity tips.";
        }

        if (normalized.Contains("thank you") || normalized.Contains("thanks"))
        {
            return $"You're welcome, {memory.UserName}. I'm always here to help.";
        }

        if (normalized.Contains("bye") || normalized.Contains("goodbye"))
        {
            return $"Goodbye, {memory.UserName}. Stay safe online.";
        }

        if (normalized.Contains("your name") || normalized.Contains("who are you"))
        {
            return "I'm FLO WRX, your cybersecurity awareness assistant.";
        }

        if (normalized.Contains("purpose") || normalized.Contains("what do you do"))
        {
            return "My purpose is to help people stay safe online and learn cybersecurity in a simple and friendly way.";
        }

        if (normalized.Contains("help") || normalized.Contains("what can i ask"))
        {
            return "You can ask me about passwords, phishing, scams, malware, privacy, safe browsing, public Wi-Fi, backups, online banking, social media, and more.";
        }

        foreach (string keyword in _keywords)
        {
            if (normalized.Contains(keyword) && _keywordReplies.TryGetValue(keyword, out ReplyMethod? replyMethod))
            {
                return replyMethod(memory, sentiment);
            }
        }

        return "I'm not sure I understand. Can you try rephrasing? You can ask about passwords, phishing, scams, privacy, malware, or safe browsing.";
    }



    private static string CleanInput(string input)
    {
        return input.Trim()
            .ToLowerInvariant()
            .Replace("?", "")
            .Replace("!", "")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("'", "")
            .Replace("\"", "");
    }



    private string GiveTopicReply(string topic, ChatMemory memory, SentimentType sentiment)
    {
        memory.LastTopic = topic;
        string opening = GetMoodOpening(sentiment);
        string memoryText = memory.FavouriteTopic == topic
            ? $"Since you're interested in {topic}, "
            : string.Empty;

        return $"{opening}{memoryText}{GetRandomResponse(topic)}";
    }



    private string AnswerFollowUp(ChatMemory memory, SentimentType sentiment)
    {
        if (string.IsNullOrWhiteSpace(memory.LastTopic))
        {
            return "I can explain more once we choose a topic. Try asking about passwords, phishing, scams, privacy, malware, or safe browsing.";
        }

        return $"{GetMoodOpening(sentiment)}Here is another tip about {memory.LastTopic}: {GetRandomResponse(memory.LastTopic)}";
    }



    private string GetRandomResponse(string topic)
    {
        if (!_topicResponses.TryGetValue(topic, out List<string>? responses))
        {
            return "Keep your accounts secure by using strong passwords, checking links carefully, and protecting your personal information.";
        }

        int index = _random.Next(responses.Count);
        return responses[index];
    }



    private static bool IsFollowUpQuestion(string normalized)
    {
        return normalized.Contains("another tip") ||
               normalized.Contains("tell me more") ||
               normalized.Contains("give me more") ||
               normalized.Contains("explain more") ||
               normalized.Contains("more details") ||
               normalized.Contains("how do i avoid it") ||
               normalized.Contains("how can i avoid it") ||
               normalized.Contains("how do i avoid that") ||
               normalized.Contains("how can i avoid that") ||
               normalized.Contains("avoid it") ||
               normalized.Contains("avoid that") ||
               normalized.Contains("what about it") ||
               normalized.Contains("what about that") ||
               normalized.Equals("more");
    }



    private static bool IsGreeting(string normalized)
    {
        return normalized == "hi" ||
               normalized == "hello" ||
               normalized == "hey" ||
               normalized == "hy";
    }



    private static bool IsPositiveReply(string normalized)
    {
        return normalized == "good" ||
               normalized == "great" ||
               normalized == "fine" ||
               normalized.Contains("i am fine") ||
               normalized.Contains("im fine") ||
               normalized.Contains("i am good") ||
               normalized.Contains("im good") ||
               normalized.Contains("doing good") ||
               normalized.Contains("doing great");
    }



    private static bool IsNegativeReply(string normalized)
    {
        return normalized == "bad" ||
               normalized == "sad" ||
               normalized == "tired" ||
               normalized.Contains("i am bad") ||
               normalized.Contains("im bad") ||
               normalized.Contains("i am sad") ||
               normalized.Contains("im sad") ||
               normalized.Contains("i am tired") ||
               normalized.Contains("im tired");
    }



    private static SentimentType DetectSentiment(string normalized)
    {
        if (normalized.Contains("worried") || normalized.Contains("scared") || normalized.Contains("afraid"))
        {
            return SentimentType.Worried;
        }

        if (normalized.Contains("curious") || normalized.Contains("interested") || normalized.Contains("want to know"))
        {
            return SentimentType.Curious;
        }

        if (normalized.Contains("frustrated") || normalized.Contains("confused") || normalized.Contains("overwhelmed"))
        {
            return SentimentType.Frustrated;
        }

        return SentimentType.Neutral;
    }



    private static string GetMoodOpening(SentimentType sentiment)
    {
        return sentiment switch
        {
            SentimentType.Worried => "It's understandable to feel worried. ",
            SentimentType.Curious => "It's great that you're curious. ",
            SentimentType.Frustrated => "I know this can feel confusing, so let's take it step by step. ",
            _ => string.Empty
        };
    }



    private bool TryRememberFavouriteTopic(string normalized, ChatMemory memory, out string topic)
    {
        topic = string.Empty;

        bool userShowsInterest = normalized.Contains("interested in") ||
                                 normalized.Contains("favourite topic") ||
                                 normalized.Contains("favorite topic") ||
                                 normalized.Contains("i like");

        if (!userShowsInterest)
        {
            return false;
        }

        foreach (string knownTopic in _topicResponses.Keys)
        {
            if (normalized.Contains(knownTopic))
            {
                memory.FavouriteTopic = knownTopic;
                topic = knownTopic;
                return true;
            }
        }

        return false;
    }
}
