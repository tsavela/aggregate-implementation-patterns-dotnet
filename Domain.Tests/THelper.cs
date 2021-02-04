using System.Collections.Generic;
using Domain.Shared.Event;

namespace Domain.Tests
{
    public class THelper {
        public static string typeOfFirst(List<IEvent> recordedEvents) {
            if (recordedEvents.Count == 0) {
                return "???";
            }

            return recordedEvents[0].GetType().Name;
        }

        public static string propertyIsNull(string property) {
            return $"PROBLEM: The {property} is null!\nHINT: Maybe you didn't apply the previous events properly!?\n";
        }

        public static string eventIsNull(string method, string expectedEvent) {
            return
                $"PROBLEM in {method}(): The recorded/returned event is NULL!\n" +
                $"HINT: Make sure you record/return a {expectedEvent} event\n\n";
        }

        public static string propertyIsWrong(string method, string property) {
            return
                $"PROBLEM in {method}(): The event contains a wrong {property}!\n" +
                $"HINT: The {property} in the event should be taken from the command!\n\n";
        }

        public static string noEventWasRecorded(string method, string expectedEvent) {
            return
                $"PROBLEM in {method}(): No event was recorded/returned!\n" +
                $"HINTS: Build a {expectedEvent} event and record/return it!\n" +
                "       Did you apply all previous events properly?\n" +
                "       Check your business logic :-)!\n\n";
        }

        public static string eventOfWrongTypeWasRecorded(string method)
        {
            return
                $"PROBLEM in {method}(): An event of the wrong type was recorded/returned!\n" +
                "HINTS: Did you apply all previous events properly?\n" +
                "       Check your business logic :-)!\n\n";
        }

        public static string noEventShouldHaveBeenRecorded(string recordedEventType)
        {
            return
                "PROBLEM: No event should have been recorded/returned!\n" +
                "HINTS: Check your business logic - this command should be ignored (idempotency)!\n" +
                "       Did you apply all previous events properly?\n" +
                $"       The recorded/returned event is of type {recordedEventType}.\n\n";
        }
    }
}
