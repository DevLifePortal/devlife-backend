# DevLife Portal – Developer Life Simulator 🧪

**DevLife Portal** is an interactive platform that unifies 6 standalone mini-projects into one fun developer-centric experience. Each module is a gamified or social feature tailored around the daily ups and downs of a developer's life.

---

## 🚀 Getting Started

Before running the project, create a `.env` file named:

```
secrets
```

And add the following content (required):

```env
DATABASE_URL=postgresql://user:pass@localhost/devlife
MONGODB_URL=mongodb://localhost:27017/devlife
REDIS_URL=redis://localhost:6379
GITHUB_CLIENT_ID=xxx
GITHUB_CLIENT_SECRET=xxx
OPENAI_API_KEY=xxx # optional
JWT_SECRET=xxx
```

> ⚠️ All keys are mandatory except `OPENAI_API_KEY` (only used for AI-powered features).

---

## 🧩 Modules Overview

### 🎰 Code Casino
Choose between two similar code snippets — one works, the other has a bug.
- Betting system with points
- Matches your tech stack
- Zodiac-based luck bonus
- Daily challenge and leaderboard

### 🔥 Code Roast
Solve coding challenges and get roasted (or praised) by AI.
- Integrated with Judge0
- AI feedback via OpenAI / Groq
- Humorous, sarcastic responses

### 🏃 Bug Chase (Game)
Endless runner game — escape bugs, deadlines and meetings.
- WebSocket-based leaderboard
- Power-ups: coffee, weekends
- Jump / duck controls

### 🔍 Code Personality Analyzer
Analyze a GitHub repo and get your developer personality.
- GitHub OAuth
- Code patterns: commits, naming, structure
- Results: personality type + shareable card

### 💑 Dev Dating Room
Tinder-like dating simulator for devs.
- Swipe & match mechanics
- Tech stack compatibility
- AI chats with pre-defined characters

### 🏃‍♂️ Escape the Meeting (Excuse Generator)
Generate creative excuses to escape boring meetings.
- Categories: Standup, Planning, Client, Team
- Types: Technical, Personal, Creative
- Believability score, Redis favorites

---

## 🛠️ Tech Stack

- **Backend:** .NET 8 + Minimal API
- **Databases:** PostgreSQL, MongoDB, Redis
- **AI:** OpenAI API (optional)
- **Realtime:** SignalR / WebSocket
- **Frontend:** React / Canvas / Swipe UI (project-dependent)

---

## 📘 Swagger Categories

All endpoints are grouped clearly:
- `Auth` – login, registration, GitHub OAuth
- `Code Arena` – snippet guessing game
- `Daily Challenge` – leaderboard & XP
- `Developer Insights` – GitHub code analysis
- `Dev Dating` – profiles, swipes, matches
- `Excuse Generator` – generate and manage excuses

---

## 🤝 Final Words

This project was made with ❤️ for devs, by devs.  
If you’re reading this — welcome to DevLife.