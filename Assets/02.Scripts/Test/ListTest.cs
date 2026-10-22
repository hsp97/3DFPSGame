using UnityEngine;
using System.Collections.Generic;

public class ListTest : MonoBehaviour
{
    // 자료구조( Data Structure)
    // 이런 데이터들을 효율적으로 관리하기 위해 저장하는 구조 CRUDS
    // C - CREATE:  생성
    // R - READ:    읽기
    // U - UPDATE:  수정
    // D - DELETE:  삭제
    // S - SEARCH:  조회
    // BREAD 라고도 함

    // 선형 자료구조  : 배열, 리스트, 큐, 스택
    // 비선형 자료구조 : 트리, 그래프, 해쉬테이블

    private void Start()
    {
        int[] ages = new int[] { 11, 22, 33, 45 };

        // 배열은 선언할 때 지정한 배열의 크기를 변경할 수 없다.
        // -> 배열에 데이터를 추가하거나 삭제하려면 새로운 배열을 생성해야 한다.
        // -> 배열은 데이터를 중간에 추가하거나 삭제하는 경우 비효율적인 과정이 필요하다.
        ages[0] = 17;
        ages[1] = 27;
        ages[2] = 37;
        ages[3] = 47;
        ages[4] = 57; // IndexOutOfRange 에러 발생

        // 이 배열의 단점을 보완하기 위해 자료구조는 동적 배열 혹은 연결 리스트를 사용하는데
        // c# 은 콜렉션 네임스페이스 안에 List로 구현해놨다.
        // 콜렉션(Collection): 컴퓨터 공학의 자료구조를 c#에서 구조화하여 클래스로 구현해 놓은 것 들

        // List -> 배열과 똑같은데 필요에 따라 크기가 동적으로 증감하는 배열

        List<int> ages2 = new List<int>() { 11, 22, 33, 45 };
        ages2[0] = 17;
        ages2[1] = 27;
        ages2[2] = 37;
        ages2[3] = 47;
        ages2[4] = 57; // 추가 됨

        ages2.Add(23);
        ages2.Insert(1, 30);
        ages2.Remove(17);
        ages2.RemoveAt(3);

        // 리스트, 스택, 큐 차이점과 장단점, 게임개발에서 언제쓰이는지 판단
    }
}