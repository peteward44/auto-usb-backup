using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;



namespace PWLib
{
	public class BackgroundWorkerThread : IDisposable
	{
		public delegate bool TaskPredicate( object userObject );
		public delegate void Task( object userObject );
		public delegate void TaskError( object userObject, Exception e );

		Thread mThread = null;
		Mutex mMutex = null;
		int mWorkerSleepTime = 0;

		class Command
		{
			TaskPredicate mPred;
			Task mTask;
			TaskError mError;
			object mUserObject;

			public Command( TaskPredicate predicate, Task task, TaskError taskError, object userObject )
			{
				mPred = predicate;
				mTask = task;
				mError = taskError;
				mUserObject = userObject;
			}

			public bool Execute()
			{
				bool dummy = false;
				return Execute( out dummy );
			}

			public bool Execute( out bool removeOneShot )
			{
				removeOneShot = false;
				try
				{
					bool predicateSuccess = true;
					if ( mPred != null )
					{
						predicateSuccess = mPred( mUserObject );
					}

					if ( mTask != null && predicateSuccess )
					{
						mTask( mUserObject );
						removeOneShot = true;
					}
				}
				catch ( Exception e )
				{
					try
					{
						if ( mError != null )
							mError( mUserObject, e );
					}
					catch ( Exception )
					{}
				}
				return false;
			}
		}

		List<Command> mPersistentCommandList = new List<Command>();
		Queue<Command> mOneShotCommandQueue = new Queue<Command>();
		
		volatile bool mDisposed = false;
		volatile bool mDisposeTriggered = false;

		public bool Disposed { get { return mDisposed; } }


		public event Task InitialisationTask;
		public event Task ShutdownTask;


		public BackgroundWorkerThread( int workerSleepTime )
		{
			mWorkerSleepTime = workerSleepTime;
			mMutex = new Mutex();
			mThread = new Thread( new ThreadStart( StartThread ) );
		}


		public void Start()
		{
			mThread.Start();
		}


		public void RegisterOneShotTask( Task task, object userObject )
		{
			RegisterOneShotTask( null, task, null, userObject );
		}

		public void RegisterOneShotTask( TaskPredicate predicate, Task task, TaskError taskError, object userObject )
		{
			lock ( mMutex )
			{
				mOneShotCommandQueue.Enqueue( new Command( predicate, task, taskError, userObject ) );
			}
		}

		public void RegisterPersistentTask( Task task, object userObject )
		{
			RegisterPersistentTask( null, task, null, userObject );
		}

		public void RegisterPersistentTask( TaskPredicate predicate, Task task, TaskError taskError, object userObject )
		{
			lock ( mMutex )
			{
				mPersistentCommandList.Add( new Command( predicate, task, taskError, userObject ) );
			}
		}


		private void StartThread()
		{
			if ( InitialisationTask != null )
				InitialisationTask( null );

			while ( !mDisposeTriggered )
			{
				if ( mPersistentCommandList.Count > 0 )
				{
					lock ( mMutex )
					{
						foreach ( Command command in mPersistentCommandList )
						{
							command.Execute();
						}
					}
				}

				while ( mOneShotCommandQueue.Count > 0 )
				{
					Command command = null;
					lock ( mMutex )
					{
						command = mOneShotCommandQueue.Dequeue();
					}
					bool removeOneShot = false;
					command.Execute( out removeOneShot );
				}

				if ( !mDisposeTriggered && mWorkerSleepTime >= 0 )
					Thread.Sleep( mWorkerSleepTime );
			}

			if ( ShutdownTask != null )
				ShutdownTask( null );

			mMutex.Close();
			mDisposed = true;
		}


		public void Dispose()
		{
			mDisposeTriggered = true;
		}
	}

}
