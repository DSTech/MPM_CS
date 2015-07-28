using System;

namespace MPM.Net.DTO {

	public class Author {
		public String Name { get; set; }
		public String Email { get; set; }

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
