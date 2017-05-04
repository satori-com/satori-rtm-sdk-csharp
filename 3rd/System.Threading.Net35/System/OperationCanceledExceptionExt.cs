using System.Threading;

namespace System {
	public class OperationCanceledExceptionExt : OperationCanceledException {
		public CancellationToken CancellationToken { private set; get; }
        public OperationCanceledExceptionExt(CancellationToken token) {
			CancellationToken = token;
		}
	}
}
