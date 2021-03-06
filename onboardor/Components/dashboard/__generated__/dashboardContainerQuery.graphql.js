/**
 * @flow
 * @relayHash dbff7e05e8bb58c15ebfcd45a15aedf0
 */

/* eslint-disable */

'use strict';

/*::
import type { ConcreteRequest } from 'relay-runtime';
export type dashboardContainerQueryVariables = {| |};
export type dashboardContainerQueryResponse = {|
  +organizations: $ReadOnlyArray<?{|
    +id: string,
    +name: string,
    +avatarUrl: string,
    +members: $ReadOnlyArray<?{|
      +id: string,
      +name: string,
      +avatarUrl: string,
      +isBeingOnboarded: boolean,
    |}>,
  |}>,
|};
*/


/*
query dashboardContainerQuery {
  organizations {
    id
    name
    avatarUrl
    members {
      id
      name
      avatarUrl
      isBeingOnboarded
    }
  }
}
*/

const node/*: ConcreteRequest*/ = (function(){
var v0 = {
  "kind": "ScalarField",
  "alias": null,
  "name": "id",
  "args": null,
  "storageKey": null
},
v1 = {
  "kind": "ScalarField",
  "alias": null,
  "name": "name",
  "args": null,
  "storageKey": null
},
v2 = {
  "kind": "ScalarField",
  "alias": null,
  "name": "avatarUrl",
  "args": null,
  "storageKey": null
},
v3 = [
  {
    "kind": "LinkedField",
    "alias": null,
    "name": "organizations",
    "storageKey": null,
    "args": null,
    "concreteType": "Organization",
    "plural": true,
    "selections": [
      v0,
      v1,
      v2,
      {
        "kind": "LinkedField",
        "alias": null,
        "name": "members",
        "storageKey": null,
        "args": null,
        "concreteType": "Member",
        "plural": true,
        "selections": [
          v0,
          v1,
          v2,
          {
            "kind": "ScalarField",
            "alias": null,
            "name": "isBeingOnboarded",
            "args": null,
            "storageKey": null
          }
        ]
      }
    ]
  }
];
return {
  "kind": "Request",
  "operationKind": "query",
  "name": "dashboardContainerQuery",
  "id": null,
  "text": "query dashboardContainerQuery {\n  organizations {\n    id\n    name\n    avatarUrl\n    members {\n      id\n      name\n      avatarUrl\n      isBeingOnboarded\n    }\n  }\n}\n",
  "metadata": {},
  "fragment": {
    "kind": "Fragment",
    "name": "dashboardContainerQuery",
    "type": "Query",
    "metadata": null,
    "argumentDefinitions": [],
    "selections": v3
  },
  "operation": {
    "kind": "Operation",
    "name": "dashboardContainerQuery",
    "argumentDefinitions": [],
    "selections": v3
  }
};
})();
(node/*: any*/).hash = '6cf885eb8ac76a0db218ee2e4072dd02';
module.exports = node;
