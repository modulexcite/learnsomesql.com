using System;
using System.Security.Cryptography;
using System.Text;

namespace RedGate.Publishing.InteractiveSql.Courses
{
    public class Question
    {
        private readonly string m_Description;
        private readonly string m_CorrectQuery;

        public Question(string description, string correctQuery)
        {
            m_Description = description;
            m_CorrectQuery = correctQuery;
        }

        public string Description
        {
            get { return m_Description; }
        }

        public string CorrectQuery
        {
            get { return m_CorrectQuery; }
        }

        public string Identify()
        {
            return Sha1(Sha1(m_Description) + Sha1(m_CorrectQuery));
        }

        private static string Sha1(string value)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(value));
            string delimitedHexHash = BitConverter.ToString(hash);
            return delimitedHexHash.Replace("-", "");
        }
    }
}