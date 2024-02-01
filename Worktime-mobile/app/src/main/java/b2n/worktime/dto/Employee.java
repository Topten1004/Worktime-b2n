package b2n.worktime.dto;

public class Employee {
    public int id;
    public String name;

    public Employee(int _id, String _name)
    {
        id = _id;
        name = _name;
    }

    @Override
    public String toString() {
        return name;
    }

    public String getName() {
        return this.name;
    }

    public int getId() {
        return this.id;
    }
}
