using System.Collections.Generic;
using UnityEngine;

public class DictionaryTest : MonoBehaviour
{
    // 배열이나 List 는 인덱스를 이용해 요소에 접근한다.
    // 하지만 딕셔너리는 요소를 저장할 때 키와 값을 쌍으로 저장한다.
    // HashTable(=Dictionary)은 키와 값이 쌍으로 이루어진 데이터를 관리할 때 사용하는 자료구조
    private void Start()
    {
        List<int> ages = new List<int>() { 22, 21, 20, 35 };
        Dictionary<string, int> students = new Dictionary<string, int>()
        {
            {
                "홍길동", 22
            },
            {
                "홍길서", 21
            },
            {
                "홍길남", 23
            },
            {
                "홍길북", 24
            }
        };

        students["홍길동"] = 21;
        students.Add("홍길동서남북", 12);
        students.Remove("홍길동");
        bool has = students.ContainsKey("홍길남");

        foreach (KeyValuePair<string, int> item in students)
        {
            Debug.Log($"{item.Key}의 나이, {item.Value}");
        }
    }
}