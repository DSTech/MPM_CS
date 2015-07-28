using System;

namespace MPM.Types {

	public class Author {

		public Author(string name, string email) {
			this.Name = name;
			this.Email = email;
		}

		public String Name { get; }
		public String Email { get; }

		public override string ToString() {
			var nameExists = !String.IsNullOrWhiteSpace(Name);
			var emailExists = !String.IsNullOrWhiteSpace(Email);
			if (nameExists) {
				if (emailExists) {
					return $"{Name} <{Email}>";
				} else {
					return Name;
				}
			} else {
				if (emailExists) {
					return $"<{Email}>";
				} else {
					return "";
				}
			}
		}
	}
}
