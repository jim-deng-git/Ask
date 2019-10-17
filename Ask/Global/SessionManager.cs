using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WorkV3.Global
{
    internal class SessionManager
    {
        private readonly HttpSessionState _session;
        private string SessionName { get; set; }

        internal SessionManager()
        {
            _session = HttpContext.Current.Session;
        }

        internal SessionManager(string sessionName) : this()
        {
            _session = HttpContext.Current.Session;
            SetSessionName(sessionName);
        }

        internal void SetSessionName(string sessionName)
        {
            SessionName = sessionName;
        }

        #region Depends on SessionKey provied using constructor
        internal bool DoesKeyExist()
        {
            if (!HasAnySessions())
            {
                return false;
            }
            bool exist = false;
            for (int i = 0; i < _session.Count; i++)
            {
                exist = _session.Keys[i] == SessionName;
                if (exist)
                {
                    break;
                }
            }
            return exist;
        }

        internal bool IsNull()
        {
            return _session[SessionName] == null;
        }

        internal void SetNull()
        {
            _session[SessionName] = null;
        }

        internal TSource Get<TSource>()
        {
            return (TSource)_session[SessionName];
        }

        internal void Set<TSource>(TSource model)
        {
            _session.Add(SessionName, model);
        }

        internal void Replace<TSource>(TSource model)
        {
            _session[SessionName] = model;
        }

        internal void Remove()
        {
            _session.Remove(SessionName);
        }
        #endregion


        internal string GetSessionId()
        {
            return _session.SessionID;
        }

        internal bool HasAnySessions()
        {
            return _session.Count > 0;
        }

        internal void RemoveAll()
        {
            _session.RemoveAll();
        }

        internal void AbandonSessions()
        {
            _session.Abandon();
        }
    }
}