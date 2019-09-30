using Domain;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Tests
{
    public class BddTest<TAggregate> where TAggregate : Aggregate, new()
    {
        private TAggregate _sut;

        [SetUp]
        public void BddTestSetup()
        {
            _sut = new TAggregate();
        }

        protected void Test(IEnumerable given, Func<TAggregate, object> when, Action<object> then)
        {
            then(when(ApplyEvents(_sut, given)));
        }

        protected IEnumerable Given(params object[] events)
        {
            return events;
        }

        protected Func<TAggregate, object> When<TCommand>(TCommand command)
        {
            return agg =>
            {
                try
                {
                    return DispatchCommand(command).Cast<object>().ToArray();
                }
                catch (Exception e)
                {
                    return e;
                }
            };
        }

        protected Action<object> Then(params object[] expectedEvents)
        {
            return got =>
            {
                switch (got)
                {
                    case object[] gotEvents when gotEvents.Length == expectedEvents.Length:
                        {
                            for (var i = 0; i < gotEvents.Length; i++)
                            {
                                if (gotEvents[i].GetType() == expectedEvents[i].GetType())
                                {
                                    Assert.AreEqual(Serialize(expectedEvents[i]), Serialize(gotEvents[i]));
                                }
                                else
                                {
                                    Assert.Fail(
                                        $"Incorrect event in results; expected a {expectedEvents[i].GetType().Name} but got a {gotEvents[i].GetType().Name}");
                                }
                            }

                            break;
                        }
                    case object[] gotEvents when gotEvents.Length < expectedEvents.Length:
                        Assert.Fail(
                            $"Expected event(s) missing: {string.Join(", ", EventDiff(expectedEvents, gotEvents))}");
                        break;
                    case object[] gotEvents:
                        Assert.Fail(
                            $"Unexpected event(s) emitted: {string.Join(", ", EventDiff(gotEvents, expectedEvents))}");
                        break;
                    case Exception exception:
                        Assert.Fail(exception.Message);
                        break;
                    default:
                        Assert.Fail($"Expected events, but got exception {got.GetType().Name}");
                        break;
                }
            };
        }

        protected Action<object> ThenFailWith<TException>()
        {
            return got =>
            {
                switch (got)
                {
                    case TException _:
                        Assert.Pass("Got correct exception type");
                        break;
                    case Exception _:
                        Assert.Fail(
                            $"Expected exception {typeof(TException).Name}, but got exception {got.GetType().Name}");
                        break;
                    default:
                        Assert.Fail($"Expected exception {typeof(TException).Name}, but got event result");
                        break;
                }
            };
        }

        private IEnumerable DispatchCommand<TCommand>(TCommand command)
        {
            if (_sut is IHandleCommand<TCommand> handler)
            {
                return handler.Handle(command);
            }

            throw new Exception(
                $"Aggregate {_sut.GetType().Name} does not yet handle command {command.GetType().Name}");
        }

        private static TAggregate ApplyEvents(TAggregate sut, IEnumerable events)
        {
            sut.ApplyEvents(events);
            return sut;
        }

        private static string[] EventDiff(IEnumerable<object> a, IEnumerable<object> b)
        {
            var diff = a.Select(e => e.GetType().Name).ToList();

            foreach (var d in b.Select(e => e.GetType().Name))
            {
                diff.Remove(d);
            }

            return diff.ToArray();
        }

        private static string Serialize(object obj)
        {
            var ser = new XmlSerializer(obj.GetType());
            var ms = new MemoryStream();

            ser.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(ms);

            return reader.ReadToEnd();
        }
    }
}
