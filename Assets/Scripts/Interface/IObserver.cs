public interface IObserver
{
    void OnNotify(); // 상태 변경 시 호출될 메서드
}

public interface ISubject
{
    void RegisterObserver(IObserver observer); // 옵저버 등록
    void RemoveObserver(IObserver observer); // 옵저버 제거
    void NotifyObservers(); // 옵저버들에게 알림
}