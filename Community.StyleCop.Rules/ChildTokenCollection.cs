// <copyright file="ChildTokenCollection.cs" company="n/a">
// Copyright 2009, 2010 Christian Schlotter
//
// This file is part of StyleCop Community Rules.
//
// StyleCop Community Rules is free software: you can redistribute it and/or
// modify it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or (at your
// option) any later version.
//
// StyleCop Community Rules is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with StyleCop Community Rules. If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <author>Christian Schlotter</author>
// <email>again@gmx.de</email>
// <summary>Allows to iterate through the lowest level of tokens contained in a
// given token list.</summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

using StyleCop;
using StyleCop.CSharp;

namespace Community.StyleCop.CSharp
{
    /// <summary>
    /// Collection for iterating through tokens. If a token contains child
    /// tokens, these will be returned instead of their parent token.
    /// </summary>
    internal class ChildTokenCollection : IEnumerable<CsToken>
    {
        /// <summary>
        /// The tokens of this collection.
        /// </summary>
        private readonly MasterList<CsToken> tokens;

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="ChildTokenCollection"/> class.
        /// </summary>
        /// <param name="tokens">The tokens that this collection exposes.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref
        /// name="tokens"/> is <c>null</c>.</exception>
        public ChildTokenCollection(MasterList<CsToken> tokens)
        {
            Param.RequireNotNull(tokens, "tokens");

            this.tokens = tokens;
        }

        #region IEnumerable<CsToken> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{CsToken}"/> that can be used to
        /// iterate through the collection.</returns>
        public IEnumerator<CsToken> GetEnumerator()
        {
            var lists = new Stack<Node<CsToken>>();

            for (Node<CsToken> node = this.tokens.First;
                node != null;
                node = GetNextNode(node, lists))
            {
                MasterList<CsToken> childTokens;
                while (HasChildTokens(node.Value, out childTokens))
                {
                    // store current node
                    lists.Push(node);

                    // make first child token the current node
                    node = childTokens.First;
                }

                CsToken token = node.Value;

                switch (token.CsTokenClass)
                {
                    // tokens that do not contain child tokens
                    case CsTokenClass.Bracket:
                    case CsTokenClass.ConditionalCompilationDirective:
                    case CsTokenClass.Number:
                    case CsTokenClass.OperatorSymbol:
                    case CsTokenClass.PreprocessorDirective:
                    case CsTokenClass.RegionDirective:
                    case CsTokenClass.Token:
                    case CsTokenClass.Whitespace:
                        yield return token;
                        continue;

                    // tokens that should have been filtered out as they
                    // contain child tokens
                    case CsTokenClass.Attribute:
                    case CsTokenClass.ConstructorConstraint:
                    case CsTokenClass.GenericType:
                    case CsTokenClass.Type:
                    case CsTokenClass.XmlHeader:
                        string message = string.Format(
                            CultureInfo.CurrentCulture,
                            "Unexpected token class '{0}'.",
                            token.CsTokenClass);
                        throw new InvalidOperationException(message);

                    // Were tokens forgotten?
                    default:
                        message = string.Format(
                            CultureInfo.CurrentCulture,
                            "Unknown token class '{0}'.",
                            token.CsTokenClass);
                        throw new InvalidOperationException(message);
                }
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to
        /// iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Determines whether the specified type has a <c>ChildTokens</c>
        /// property that returns a <see cref="MasterList{CsToken}"/>.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="childTokens">If available, the value of the
        /// <c>ChildTokens</c> property; otherwise <c>null</c>.</param>
        /// <returns><c>true</c> if the <paramref name="token"/> has a
        /// <c>ChildTokens</c> property and <paramref name="childTokens"/> is
        /// not <c>null</c>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref
        /// name="token"/> is <c>null</c>.</exception>
        private static bool HasChildTokens(
            CsToken token,
            out MasterList<CsToken> childTokens)
        {
            Param.RequireNotNull(token, "token");

            Type expectedReturnType = typeof(MasterList<CsToken>);
            PropertyInfo propertyInfo = token.GetType().GetProperty(
                Strings.ChildTokens,
                expectedReturnType);

            if (propertyInfo != null)
            {
                childTokens = (MasterList<CsToken>)propertyInfo.GetValue(
                    token,
                    null);
            }
            else
            {
                childTokens = null;
            }

            return childTokens != null;
        }

        /// <summary>
        /// Gets the next node.
        /// </summary>
        /// <param name="node">A node of a linked list which contains tokens.
        /// </param>
        /// <param name="tokenListStack">The stack containing linked lists
        /// which contain tokens.</param>
        /// <returns>Returns <paramref name="node.Next"/> if it is not
        /// <c>null</c>; otherwise returns the <c>Next</c> property of top
        /// <paramref name="tokenListStack"/> node if it has a positive number
        /// of elements; otherwise returns <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref
        /// name="node"/> or <paramref name="tokenListStack"/> is <c>null</c>.
        /// </exception>
        private static Node<CsToken> GetNextNode(
            Node<CsToken> node,
            Stack<Node<CsToken>> tokenListStack)
        {
            Param.RequireNotNull(node, "node");
            Param.RequireNotNull(tokenListStack, "tokenListStack");

            if (node.Next != null)
            {
                return node.Next;
            }

            if (tokenListStack.Count > 0)
            {
                return tokenListStack.Pop().Next;
            }

            return null;
        }
    }
}
