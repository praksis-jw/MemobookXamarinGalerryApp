using Memobook.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Memobook.Data
{
    public class EventDatabaseController
    {
        static object locker = new object();

        SQLiteConnection database;
        public EventDatabaseController()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            database.CreateTable<Event>();
            database.CreateTable<EventUser>();
        }
        public Event GetEvent()
        {
            lock(locker)
            {
                if(database.Table<Event>().Count() ==0)
                { return null; }
                else
                {
                    return database.Table<Event>().First();
                }
            }
        }
        public int SaveEvent(Event eventt)
        {
            lock(locker)
            {
                if(eventt.EventIdAuto !=0)
                {
                    database.Update(eventt);
                    return eventt.EventIdAuto;
                }
                else
                {
                    
                    database.Insert(eventt);
                    database.Commit();
                    return 1;
                }
            }

        }

        public Guid SaveUser(EventUser eventuser)
        {
            lock (locker)
            {
                if (eventuser.EventUserID != Guid.Empty)
                {
                    database.Update(eventuser);
                    return eventuser.EventUserID;
                }
                else
                {

                    database.Insert(eventuser);
                    database.Commit();
                    return eventuser.EventUserID;
                }
            }

        }

        public int DeleteEvent(int id)
        {
            lock (locker)
            {
                return database.Delete<Event>(id);
            }
        }
    }
}
