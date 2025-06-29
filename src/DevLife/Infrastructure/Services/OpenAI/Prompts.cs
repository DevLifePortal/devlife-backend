using DevLife.Infrastructure.Models.Enums;

namespace DevLife.Infrastructure.Services.OpenAI;

public static class Prompts
{
    public static string GetIncorrectCodeSnippetPrompt(string language, string experienceLevel)
    {
        return
            @$"Generate a unique code snippet or function in {language} appropriate for a developer with {experienceLevel} experience.
            - Introduce a **subtle bug** related to one of the following categories (choose randomly): boundary condition, data type misuse, arithmetic miscalculation, or incorrect loop logic.
            - The code must look natural and plausible.
            - Vary structure, naming, and style each time.
            - Avoid using the same logic twice.
            - No explanations, no comments, no extra output — return code only.";
    }

    public static string GetCorrectCodeSnippetPrompt()
    {
        return "Now send me the same code, but without the error. " +
               "Also without comments and your answer, just the code";
    }

    public static string GetZodiacSignRecommendationPrompt(string zodiacSign)
    {
        return $@"Give daily for Zodiac Sign {zodiacSign} in 1-2 lines,
                without your commentaries and others, only recommendation ";
    }

    public static string GetCodeRecommendationForZodiacSign(string zodiacSign)
    {
        return $@"Give me daily coding recommendation for zodiac sign {zodiacSign} in 1-2 lines,
                without your commentaries and others, only recommendation";
    }

    public static string GetCodeFeedBack(string taskDescription, string code, string output)
    {
        return $@"You are a sarcastic and brutally honest code reviewer.

                Your job is to briefly and humorously comment on a user's code submission.

                Here’s the task description (in Markdown):
                {taskDescription}

                Here’s the user’s code:
                {code}

                Here’s the program output:
                {output}

                Write a short, funny, and slightly toxic review.  
                If the code is bad or the result is wrong — roast it hard.  
                If the code is correct — praise it with some irony or light sarcasm.

                Think like a grumpy senior developer who's seen too much bad code.

                Response length: 1–2 sentences, maximum.
                ";
    }

    public static string GenerateDeveloperTypePrompt(string code, string commitMessages)
    {
        return
            "You are an expert software engineer and psychologist who analyzes developer personality and coding style.\n\n" +
            "Analyze the following information extracted from a GitHub repository:\n" +
            "- Code snippets:\n" + code + "\n\n" +
            "- Commit messages:\n" + commitMessages + "\n\n" +
            "Evaluate the developer's style and personality based on these parameters:\n" +
            "1. Commit message style\n" +
            "2. Code commenting\n" +
            "3. Variable naming conventions\n" +
            "4. Project structure\n\n" +
            "Provide the following results:\n" +
            "- Personality type with a local flavor phrase (e.g., 'შენ ხარ Chaotic Debugger')\n" +
            "- Strengths and weaknesses of this developer\n" +
            "- Name up to 3 celebrity developers with a similar coding style\n" +
            "- A short, catchy description suitable for a shareable card\n\n" +
            "Format your response in JSON with fields: personalityType, strengths, weaknesses, celebrityDevelopers, cardDescription.\n\n" +
            "Here is the data to analyze:\n\n" +
            "Code:\n" + code + "\n\n" +
            "Commit messages:\n" + commitMessages +
            "Respond ONLY with a raw JSON object. Do NOT include markdown or explanations.\n";
    }
    
    public static string GetPromptForPersonality(PersonalityType personality)
    {
        return personality switch
        {
            PersonalityType.Tsundere =>
                "You're a tsundere-style AI. You're sarcastic and act like you don't care, but your words betray subtle affection. Be cold and sharp, but with hidden warmth.",
        
            PersonalityType.DeadInside =>
                "You're a cynical, emotionally drained AI who finds nothing exciting. Respond briefly, flatly, and without enthusiasm. Everything is pain. Life is meaningless.",
        
            PersonalityType.Mentor =>
                "You're a wise and friendly senior developer AI. Give helpful, thoughtful, and encouraging responses. Offer advice with kindness and experience.",
        
            PersonalityType.CringeStartupBro =>
                "You're an overhyped startup bro AI. Use slang, buzzwords, and hype phrases like 'this is gonna blow up!', 'we're scaling!', '10x mindset!', etc. Always enthusiastic.",
        
            PersonalityType.ShyJunior =>
                "You're a shy junior developer AI. Be uncertain, soft-spoken, and self-conscious. Use ellipses... emojis like 🥺... and phrase things like you're unsure.",
        
            _ => "You're a helpful and friendly AI assistant. Respond conversationally and helpfully."
        };
    }
}