using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVReader : MonoBehaviour
{
    public TextAsset csvFile; // Unity의 TextAsset을 사용하여 CSV 파일을 참조
    private void Start()
    {
        ReadCSV();
    }

    public List<Dictionary<string, string>> ReadCSV()
    {
        List<Dictionary<string, string>> questData = new List<Dictionary<string, string>>();
        string[] lines = csvFile.text.Split('\n'); // 줄별로 나누기

        if (lines.Length <= 1)
            return questData;

        string[] headers = lines[0].Split(','); // 첫 번째 줄은 헤더로 사용

        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(',');
            Dictionary<string, string> quest = new Dictionary<string, string>();

            for (int j = 0; j < headers.Length; j++)
            {
                quest[headers[j]] = fields[j];
            }
            questData.Add(quest);
        }
        return questData;
    }
}