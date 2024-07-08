namespace TicketingSystem.Enums {
    public enum Priority {
        P1 = 1,
        P2 = 2,
        P3 = 3,
        P4 = 4
    }

    public enum ReportSource {
        INTERNAL, CUSTOMER
    }
    public enum TicketState {
        NEW, WORKING, CUSTOMERONHOLD, RESOLVED, CLOSED, NONE, VOID
    }
    public enum TicketActivityEnum {
        CREATE, CUSTOMERONHOLD, RESOLVE, CLOSE, VOID, MODIFY, COMMENT, STARTWORK
    }
    public enum InternalFlag {
        CUSTOMER = 0, INTERNAL = 99
    }
}
