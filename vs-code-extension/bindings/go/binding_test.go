package tree_sitter_birch_test

import (
	"testing"

	tree_sitter "github.com/tree-sitter/go-tree-sitter"
	tree_sitter_birch "github.com/tree-sitter/tree-sitter-birch/bindings/go"
)

func TestCanLoadGrammar(t *testing.T) {
	language := tree_sitter.NewLanguage(tree_sitter_birch.Language())
	if language == nil {
		t.Errorf("Error loading Birch grammar")
	}
}
