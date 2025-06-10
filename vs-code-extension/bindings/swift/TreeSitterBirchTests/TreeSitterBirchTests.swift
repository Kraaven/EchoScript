import XCTest
import SwiftTreeSitter
import TreeSitterBirch

final class TreeSitterBirchTests: XCTestCase {
    func testCanLoadGrammar() throws {
        let parser = Parser()
        let language = Language(language: tree_sitter_birch())
        XCTAssertNoThrow(try parser.setLanguage(language),
                         "Error loading Birch grammar")
    }
}
