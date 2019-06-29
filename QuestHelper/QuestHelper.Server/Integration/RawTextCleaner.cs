using System;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Server.Integration
{
    /// <summary>
    /// Преобразователь сырого распознанного текста в текст с предложениями и пунктуацией
    /// </summary>
    public class RawTextCleaner
    {
        public RawTextCleaner()
        {
        }

        public string Clean(string rawText)
        {
            string resultText = string.Empty;
            string[] sentencesArray;
            if (rawText.Contains("dot"))
                sentencesArray = rawText.Split("dot");
            else if (rawText.Contains("точка"))
                sentencesArray = rawText.Split("точка");
            else sentencesArray = new string[1]{rawText};

            foreach (string sentence in sentencesArray)
            {
                resultText += " " + ClearRawSentence(sentence);
            }
            return resultText.Trim();
        }

        private string ClearRawSentence(string sentence)
        {
            var wordsArray = sentence.Split(' ');
            List<StringBuilder> textSequences = new List<StringBuilder>();
            foreach (string rawWord in wordsArray)
            {
                string word = rawWord.Trim();
                if(word.Length > 0)
                {
                    if (textSequences.Count == 0)
                    {
                        StringBuilder sbSequence = new StringBuilder();
                        string firstWordInSequence = word.Substring(0, 1).ToUpperInvariant();
                        if (word.Length > 1)
                        {
                            firstWordInSequence += word.Substring(1, word.Length - 1);
                        }
                        sbSequence.Append(firstWordInSequence);
                        textSequences.Add(sbSequence);
                    }
                    else
                    {
                        StringBuilder sbSequence = textSequences[textSequences.Count - 1];
                        switch (word)
                        {
                            case "запятая":
                            case "comma":
                                {
                                    sbSequence.Append(",");
                                }; break;
                            case "дефис":
                            case "тире":
                            case "hyphen":
                                {
                                    sbSequence.Append(" -");
                                }; break;
                            default:
                                {
                                    sbSequence.Append(" " + word);
                                }; break;
                        }
                    }
                }
            }

            string result = string.Empty;
            foreach (var sequence in textSequences)
            {
                result += sequence.ToString();
            }

            if (result.Trim().Length > 0) result += ".";

            return result;
        }
    }
}
