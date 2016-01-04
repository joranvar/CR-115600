.PHONY: all
all: test

MAKE_binDir     ?= bin

FSHARP_fsc      ?= env fsharpc
FSHARP_fsi      ?= env fsharpi
FSHARP_binDir   ?= $(MAKE_binDir)
include Makefiles/FSharp.mk

TEST_diff    ?= env diff
TEST_testDir ?= test
include Makefiles/Test.mk

OUTPUT    = $(call FSHARP_mkScriptTarget,6.fsx)
TEST      = $(call TEST_mkCompareTarget,$(OUTPUT))

.PHONY: all
all: test

.PHONY: test
test: $(TEST)

.PHONY: gold
gold: $(call TEST_mkGoldTarget,$(TEST))

.PHONY: clean
clean: cleanall
