using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.IO;
using System.Security.Permissions;

namespace Gobiner.SecureRepl
{
	public class ConsoleCapturer : MarshalByRefObject
	{
		private TextWriter OldConsole;
		private StringBuilder CapturedText = new StringBuilder();
		[SecurityCritical]
		public ConsoleCapturer()
		{
			new PermissionSet(PermissionState.Unrestricted).Assert();
			OldConsole = Console.Out;
			CodeAccessPermission.RevertAssert();
		}

		[SecurityCritical]
		public void StartCapture()
		{
			new PermissionSet(PermissionState.Unrestricted).Assert();
			Console.SetOut(new LimitedStringWriter(100000, CapturedText));
			CodeAccessPermission.RevertAssert();
		}

		[SecurityCritical]
		public void StopCapture()
		{
			new PermissionSet(PermissionState.Unrestricted).Assert();
			Console.SetOut(OldConsole);
			CodeAccessPermission.RevertAssert();
		}

		public string[] GetCapturedLines()
		{
			var @string = CapturedText.ToString();
			if (@string.EndsWith(Environment.NewLine))
			{
				@string = @string.Substring(0, @string.Length - Environment.NewLine.Length);
			}
			return @string.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
		}

		public void Clear()
		{
			CapturedText.Clear();
		}

		private class LimitedStringWriter : StringWriter
		{
			private long limit;
			public long Limit { get { return limit; } }

			public LimitedStringWriter(long limit) : base() { this.limit = limit; }
			public LimitedStringWriter(long limit, IFormatProvider formatProvider) : base(formatProvider) { this.limit = limit; }
			public LimitedStringWriter(long limit, StringBuilder sb) : base(sb) { this.limit = limit; }
			public LimitedStringWriter(long limit, StringBuilder sb, IFormatProvider formatProvider) : base(sb, formatProvider) { this.limit = limit; }


			[System.Runtime.TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
			public override void Write(char value)
			{
				if (this.GetStringBuilder().Length < limit)
				{
					base.Write(value);
				}
			}
			[System.Runtime.TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
			public override void Write(string value)
			{
				if (this.GetStringBuilder().Length < limit)
				{
					base.Write(value);
				}
			}
			public override void Write(char[] buffer, int index, int count)
			{
				if (this.GetStringBuilder().Length < limit)
				{
					base.Write(buffer, index, count);
				}
			}
		}
	}
}
