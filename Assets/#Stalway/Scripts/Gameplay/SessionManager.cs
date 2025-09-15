using Sirenix.OdinInspector;
using System;
using Unity.Burst;
using UnityEngine;

namespace Breaddog.Gameplay
{
    /// <summary>
    /// ��������� �������, � ����� ����� ���������� ���������.
    /// </summary>
    public class SessionManager : SerializedMonoBehaviour
    {
        [SerializeField] private Session defaultSession;

        // ������ SessionManager �� ������ ���������, ������� �� ���������� ��� �������� ����������
        private PoolManager poolManager;
        private Session session;

        /// <summary>
        /// �������������� ��� ������� ���������
        /// </summary>
        public void Initialize()
        {

        }

        /// <summary>
        /// �������������� ��� ������ � ��� ��������� � ��� ���������
        /// </summary>
        public void InitializeSession(Session session = null)
        {
            session ??= defaultSession;
            session.Initialize();

            this.session = session;
        }
    }

    [Serializable, BurstCompile]
    public class Session
    {
        public void Initialize()
        {

        }
    }
}
