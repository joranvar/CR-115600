TESTDIR ?= test/

.PHONY: all
all: test

.PHONY: test
test: $(TESTDIR)test6.success

$(TESTDIR)test6.success: 6.fsx
	env fsharpi $< | diff $(TESTDIR)test6.expect && touch $@

.PHONY: expect
expect: 6.fsx
	env fsharpi $< > $(TESTDIR)test6.expect

vpath %.fs src
vpath %.fsx src
