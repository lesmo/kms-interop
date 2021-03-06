﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using KMS.Interop.OAuth.Utils;

namespace Kms.Interop.OAuth.Utils {

    public static class NameValueCollectionExpansion {

        public static NameValueCollection SortByName(this NameValueCollection thisCollection) {
            var sortedCollection = new NameValueCollection();
            IOrderedEnumerable<string> keys = thisCollection.AllKeys.OrderBy(b => b, new LexicographicalComparer());

            foreach ( string key in keys ) {
                IOrderedEnumerable<string> values = thisCollection.GetValues(key)
                                                                  .OrderBy(b => b, new LexicographicalComparer());

                if ( values == null ) {
                    sortedCollection.Add(key, "");
                } else {
                    foreach ( string value in values )
                        sortedCollection.Add(key, value);
                }
            }

            return sortedCollection;
        }

        public static void AddFromDictionary(
            this NameValueCollection thisCollection,
            Dictionary<string, string> dictionary) {
            AddFromDictionary<string, string>(thisCollection, dictionary);
        }

        public static void AddFromDictionary<T1, T2>(
            this NameValueCollection thisCollection,
            Dictionary<T1, T2> dictionary) {
            foreach ( var item in dictionary )
                thisCollection.Add(item.Key.ToString(), item.Value.ToString());
        }

        public static Dictionary<string, string> ToDictionary(this NameValueCollection thisCollection, bool dummy) {
            var returnDictionary = new Dictionary<string, string>();

            foreach ( string key in thisCollection.AllKeys ) {
                foreach ( string value in thisCollection.GetValues(key) )
                    returnDictionary.Add(key, value);
            }

            return returnDictionary;
        }
    }

}