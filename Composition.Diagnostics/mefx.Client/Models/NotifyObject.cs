namespace mefx.Client.Models
{
	using System;
	using System.ComponentModel;
	using System.Diagnostics;

	/// <summary>
	/// Implements the INotifyPropertyChanged interface and provides
	/// a RaisePropertyChanged method for derived classes to use.
	/// </summary>
	public abstract class NotifyObject : INotifyPropertyChanged
	{
		#region Events
		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged
		{
			add { this._propertyChanged += value; }
			remove { this._propertyChanged -= value; }
		}

		/// <summary>
		/// The private event.
		/// </summary>
		private event PropertyChangedEventHandler _propertyChanged = delegate { };
		#endregion

		#region Methods
		/// <summary>
		/// Raises the property changed event for the given property.
		/// </summary>
		/// <param name="property">The property that is raising the event.</param>
		protected void RaisePropertyChanged(string property)
		{
			this.RaisePropertyChanged(property, true);
		}

		/// <summary>
		/// Raises the property changed event for the given property.
		/// </summary>
		/// <param name="property">The property that is raising the event.</param>
		/// <param name="verifyProperty">if set to <c>true</c> the property should be verified.</param>
		protected void RaisePropertyChanged(string property, bool verifyProperty)
		{
			if (verifyProperty)
			{
				this.VerifyProperty(property);
			}

			var handler = this._propertyChanged;

			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(property));
			}
		}

		/// <summary>
		/// Verifies that the given property exists on the object.
		/// </summary>
		/// <param name="property">The property name to verify.</param>
		[Conditional("DEBUG")]
		private void VerifyProperty(string property)
		{
			var type = GetType();

			var propertyInfo = type.GetProperty(property);

			if (propertyInfo == null)
			{
				var message = String.Format(
					"'{0}' is not a property of {1}",
					property,
					type.FullName);

				throw new InvalidOperationException(message);
			}
		}
		#endregion
	}
}
