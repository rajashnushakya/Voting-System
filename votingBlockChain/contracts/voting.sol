// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract Voting {
    struct Candidate {
        string name;
        uint256 votes;
    }

    mapping(uint256 => Candidate) public candidates;
    uint256 public candidateCount;

    constructor(string[] memory _names) {
        for (uint256 i = 0; i < _names.length; i++) {
            candidates[i] = Candidate(_names[i], 0);
        }
        candidateCount = _names.length;
    }

    function vote(uint256 index) public {
        candidates[index].votes++;
    }

    function getCandidate(uint256 index) public view returns (string memory, uint256) {
        return (candidates[index].name, candidates[index].votes);
    }
}
