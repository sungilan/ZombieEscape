public interface IObserver
{
    void OnNotify(); // ���� ���� �� ȣ��� �޼���
}

public interface ISubject
{
    void RegisterObserver(IObserver observer); // ������ ���
    void RemoveObserver(IObserver observer); // ������ ����
    void NotifyObservers(); // �������鿡�� �˸�
}