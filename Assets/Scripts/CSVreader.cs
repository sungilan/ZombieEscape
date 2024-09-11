using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVReader : MonoBehaviour
{
    public TextAsset csvFile; // Unity�� TextAsset�� ����Ͽ� CSV ������ ����
    private void Start()
    {
        ReadCSV();
    }

    public List<Dictionary<string, string>> ReadCSV()
    {
        List<Dictionary<string, string>> questData = new List<Dictionary<string, string>>();
        string[] lines = csvFile.text.Split('\n'); // �ٺ��� ������

        if (lines.Length <= 1)
            return questData;

        string[] headers = lines[0].Split(','); // ù ��° ���� ����� ���

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