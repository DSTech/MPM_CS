using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace MPM.Types.Mapping {

	public class Mappings {

		public static void CreateStaticMappings() {
			Mapper.AddProfile<DTOMappings>();
		}
	}

	public static class MapHelperExtensions {

		public static MapHelper<SRC, DEST> Helper<SRC, DEST>(this IMappingExpression<SRC, DEST> expression) => new MapHelper<SRC, DEST>(expression);
	}

	public class MapHelper<SRC, DEST> {

		private struct MapOperation {
			public System.Linq.Expressions.Expression<Func<DEST, object>> Dest;
			public System.Linq.Expressions.Expression<Func<SRC, object>> Src;
		}

		public MapHelper(IMappingExpression<SRC, DEST> expression) {
			this.Expression = expression;
			this.Operations = new MapOperation[0];
		}

		private MapHelper(IMappingExpression<SRC, DEST> expression, IEnumerable<MapOperation> operations) {
			this.Expression = expression;
			this.Operations = operations.ToArray();
		}

		public readonly IMappingExpression<SRC, DEST> Expression;

		private readonly IReadOnlyList<MapOperation> Operations;

		public MapHelper<SRC, DEST> OneToOne(System.Linq.Expressions.Expression<Func<DEST, object>> first, System.Linq.Expressions.Expression<Func<SRC, object>> second) {
			return new MapHelper<SRC, DEST>(Expression, Enumerable.Concat(new[] { new MapOperation { Dest = first, Src = second } }, Operations));
		}

		public MapHelper<SRC, DEST> ApplyCustom(System.Linq.Expressions.Expression<Func<DEST, object>> first, Action<IMemberConfigurationExpression<SRC>> confExpr) {
			return Apply().Expression.ForMember(first, confExpr).Helper();
		}

		public MapHelper<SRC, DEST> ApplyOneWay(System.Linq.Expressions.Expression<Func<DEST, object>> first, System.Linq.Expressions.Expression<Func<SRC, object>> second) {
			var curr = Apply().Expression.ForMember(first, opts => opts.MapFrom(second));
			return curr.Helper();
		}

		public MapHelper<SRC, DEST> IgnoreSource(System.Linq.Expressions.Expression<Func<SRC, object>> sourceSelector) {
			return Apply().Expression.ForSourceMember(sourceSelector, opts => opts.Ignore()).Helper();
		}

		public MapHelper<SRC, DEST> Ignore(System.Linq.Expressions.Expression<Func<DEST, object>> destSelector) {
			return Apply().Expression.ForMember(destSelector, opts => opts.Ignore()).Helper();
		}

		public MapHelper<SRC, DEST> Apply() {
			var current = Expression;
			foreach (var operation in Operations) {
				current = current.ForMember(operation.Dest, opts => opts.MapFrom(operation.Src));
			}
			return current.Helper();
		}

		public MapHelper<DEST, SRC> ApplyReverse() {
			var current = Expression.ReverseMap();
			foreach (var operation in Operations) {
				current = current.ForMember(operation.Src, opts => opts.MapFrom(operation.Dest));
			}
			return current.Helper();
		}

		public MapHelper<DEST, SRC> ApplyTwoWay() {
			var current = Expression;
			foreach (var operation in Operations) {
				current = current.ForMember(operation.Dest, opts => opts.MapFrom(operation.Src));
			}
			var currentRev = current.ReverseMap();
			foreach (var operation in Operations) {
				currentRev = currentRev.ForMember(operation.Src, opts => opts.MapFrom(operation.Dest));
			}
			return currentRev.Helper();
		}

		public MapHelper<DEST, SRC> ApplyTwoWay(Func<IMappingExpression<SRC, DEST>, IMappingExpression<SRC, DEST>> toConversion, Func<IMappingExpression<DEST, SRC>, IMappingExpression<DEST, SRC>> fromConversion) {
			var current = Expression;
			foreach (var operation in Operations) {
				current = current.ForMember(operation.Dest, opts => opts.MapFrom(operation.Src));
			}
			var currentRev = (toConversion != null ? toConversion : x => x)(current).ReverseMap();
			foreach (var operation in Operations) {
				currentRev = currentRev.ForMember(operation.Src, opts => opts.MapFrom(operation.Dest));
			}
			return (fromConversion != null ? fromConversion : x => x)(currentRev).Helper();
		}

		public MapHelper<DEST, SRC> ApplyTwoWay(Func<IMappingExpression<SRC, DEST>, IMappingExpression<SRC, DEST>> toConversion) => ApplyTwoWay(toConversion: toConversion, fromConversion: null);

		public MapHelper<DEST, SRC> ApplyTwoWay(Func<IMappingExpression<DEST, SRC>, IMappingExpression<DEST, SRC>> fromConversion) => ApplyTwoWay(toConversion: null, fromConversion: fromConversion);

		public MapHelper<DEST, SRC> Reverse() => this.Expression.ReverseMap().Helper();
	}
}
