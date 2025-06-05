# TeleBrief

TeleBrief is a CLI tool that automatically summarizes content from multiple Telegram channels into a concise,
well-organized brief. It uses AI to process messages, remove noise, and present only meaningful facts grouped by
categories.

## Features

- 🤖 AI-powered summarization using Semantic Kernel
- 📱 Fetches messages from configured Telegram channels
- 🗂️ Groups related information into 3-6 clear categories
- 🚫 Filters out noise, speculation, ads, and irrelevant content
- 📊 Presents information in a clean, structured format
- 📈 Tracks state of important topics over time
- ⚡ Smart caching of summaries for faster responses

## Commands

- `news` - Get today's news summary from configured channels (cached for 1 hour)
- `beat` - Check the current state of monitored topics

## Performance

The tool implements smart caching to improve performance:

- News summaries are cached for 1 hour
- Subsequent requests within the cache window return instantly
- Cache is automatically invalidated after 1 hour
- New summaries are generated only when needed

## Configuration

The application requires two configuration files:

1. `app.settings.json` (required) - Main application settings
2. `local.settings.json` (optional) - Local environment overrides

Example configuration:

```json
{
  "TelegramChannels": [
    "channel1",
    "channel2"
  ],
  "BatchSize": 10,
  "Topics": [
    {
      "Name": "Global Economic Stability",
      "Description": "Track global economic conditions, market stability, and financial risks.",
      "AnalysisPrompt": "Consider factors like: market volatility, inflation rates, currency stability, trade relations, and economic growth indicators. Higher numbers indicate increased economic risks and instability."
    }
  ]
}
```

### Topic Monitoring

TeleBrief can track the state of important topics over time. Each topic:

- Has a state value from 1 (best) to 100 (worst)
- Maintains historical context between runs
- Provides analysis of state changes
- Shows visual indicators with color-coded severity:
    - 🟢 Green (1-30): Good/Stable situation
    - 🟡 Yellow (31-70): Moderate concern
    - 🔴 Red (71-100): Critical situation

## Usage

Get today's news summary:

```bash
dotnet run -- news
```

Check topic states:

```bash
dotnet run -- beat
```

## Output Format

### News Summary

The tool generates summaries in a structured format:

```
Category Name:
- Fact 1
- Fact 2
- Fact 3

Another Category:
- Fact A
- Fact B
```

### Topic Analysis

```
Topic Analysis:

┌─────────────┬──────┬────────────────┐
│ Topic       │ ┌──┐ │ Analysis      │
│             │ │25│ │ Description   │
│             │ └──┘ │ of changes    │
└─────────────┴──────┴────────────────┘
```

## License

This project is open source and available under the MIT license.
